using BucStop.Models;
using BucStop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;



/*
 * This file handles the links to each of the game pages.
 */
namespace BucStop.Controllers
{

    public class GamesController : Controller
    {
        private readonly MicroClient _httpClient;

        private readonly SubmissionClient _submissionClient;
        private readonly PlayCountManager _playCountManager;
        private readonly ILogger<GamesController> _logger;

        public GamesController(MicroClient microClient, SubmissionClient submissionClient, IWebHostEnvironment webHostEnvironment, ILogger<GamesController> logger)
        {
            _httpClient = microClient;
            _submissionClient = submissionClient;
            _logger = logger;
            _playCountManager = new PlayCountManager(_httpClient.GetGamesList() ?? new List<Game>(), webHostEnvironment);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> IndexAsync()
        {
            _logger.LogInformation("Games index page accessed.");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Game> games = _httpClient.GetGamesList();

            foreach (Game game in games)
            {
                game.PlayCount = _playCountManager.GetPlayCount(game.Id);
            }

            games.Sort((x, y) => x.Id.CompareTo(y.Id));
            stopwatch.Stop();

            _logger.LogInformation("{Category}: Games Page Loaded in {LoadTime}ms.", "PageLoadTimes", stopwatch.ElapsedMilliseconds);
            _logger.LogInformation("{Category}: {User} accessed the games index page.", "UserActivity", User.Identity?.Name ?? "Anonymous");

            return View(games);
        }

        public async Task<IActionResult> Play(int id)
        {
            _logger.LogInformation("{Category}: User requested to play game with ID {GameId}.", "GameSuccess", id);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Game> games = _httpClient.GetGamesList();

            Game game = games.FirstOrDefault(x => x.Id == id);
            if (game == null)
            {
                _logger.LogWarning("{Category}: Game with ID {GameId} not found.", "GameSuccess", id);
                return NotFound();
            }

            _logger.LogInformation("Loading game URL: {GameUrl}", game.Content);
            _playCountManager.IncrementPlayCount(id);

            int playCount = _playCountManager.GetPlayCount(id);
            game.PlayCount = playCount;

            _logger.LogInformation("{Category}: Game '{GameTitle}' (ID: {GameId}) successfully loaded.",
                                    "GameSuccess", game.Title, game.Id);
            _logger.LogInformation("{Category}: {User} started playing '{GameTitle}' (ID: {GameId}).",
                                    "UserActivity", User.Identity?.Name ?? "Anonymous", game.Title, game.Id);

            stopwatch.Stop();
            _logger.LogInformation("{Category}: {GameTitle} Page Loaded in {LoadTime}ms.", "PageLoadTimes", game.Title, stopwatch.ElapsedMilliseconds);

            return View(game);
        }

        public async Task<IActionResult> Test(int id)
        {
            _logger.LogInformation("{Category}: Admin requested to test game with ID {GameId}.", "GameSuccess", id);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Game> games = _submissionClient.GetSubmissionsList();

            Game game = games.FirstOrDefault(x => x.Id == id);
            if (game == null)
            {
                _logger.LogWarning("{Category}: Submission with ID {GameId} not found.", "GameSuccess", id);
                return NotFound();
            }

            _logger.LogInformation("Loading submission URL: {GameUrl}", game.Content);
            _playCountManager.IncrementPlayCount(id);

            int playCount = _playCountManager.GetPlayCount(id);
            game.PlayCount = playCount;

            _logger.LogInformation("{Category}: Game '{GameTitle}' (ID: {GameId}) successfully loaded.",
                                    "GameSuccess", game.Title, game.Id);
            _logger.LogInformation("{Category}: {User} started playing '{GameTitle}' (ID: {GameId}).",
                                    "UserActivity", User.Identity?.Name ?? "Anonymous", game.Title, game.Id);

            stopwatch.Stop();
            _logger.LogInformation("{Category}: {GameTitle} Page Loaded in {LoadTime}ms.", "PageLoadTimes", game.Title, stopwatch.ElapsedMilliseconds);

            return View(game);
        }

        public IActionResult Snake()
        {
            return View();
        }

        public IActionResult Tetris()
        {
            return View();
        }

        // API endpoint for validating the form submission before actual submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateSuggestion(string title, string author, string description,
            string howToPlay, string thumbnailUrl, IFormFile jsFile)
        {
            var validationResult = await ValidateGameSubmissionForm(title, author, description,
                howToPlay, thumbnailUrl, jsFile);
            return Json(validationResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSuggestion(string username, string title, string author,
            string description, string howToPlay, string thumbnailUrl, IFormFile jsFile)
        {
            // Validate the form submission
            var validationResult = await ValidateGameSubmissionForm(title, author, description,
                howToPlay, thumbnailUrl, jsFile);

            if (!validationResult.IsValid)
            {
                TempData["Message"] = validationResult.ErrorMessage;
                TempData["ValidationErrors"] = JsonSerializer.Serialize(validationResult.Errors);
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Read JavaScript file content if provided
                string jsCodeContent = "";
                if (jsFile != null && jsFile.Length > 0)
                {
                    using var reader = new StreamReader(jsFile.OpenReadStream());
                    jsCodeContent = await reader.ReadToEndAsync();
                }

                // Create submission model
                var submissionModel = new GameSubmissionModel
                {
                    Username = username?.Trim() ?? "Anonymous",
                    SuggestedTitle = title?.Trim() ?? "",
                    SuggestedAuthor = author?.Trim() ?? "",
                    SuggestedDescription = description?.Trim() ?? "",
                    SuggestedHowTo = howToPlay?.Trim() ?? "",
                    RawJsCodeContent = jsCodeContent.Trim(),
                    SuggestedThumbnailUrl = thumbnailUrl?.Trim() ?? ""
                };

                // SECURITY WARNING: NEVER execute RawJsCodeContent directly. 
                // Save to a secure location for manual review.

                // Sanitize user input before logging to prevent log forging
                var safeUsername = submissionModel.Username?
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty);

                var safeTitle = submissionModel.SuggestedTitle?
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty);

                _logger.LogInformation("New game suggestion received from {User}: {Title}",
                    safeUsername,
                    safeTitle);
                // TODO: Save submissionModel to database or secured file store --------------------------
                // Define the Docker-mounted directory path
                // Define the Docker-mounted directory path
                var submissionDirectory = "/app/Submissions";

                // Ensure base directory exists (defense in depth)
                if (!Directory.Exists(submissionDirectory))
                {
                    Directory.CreateDirectory(submissionDirectory);
                }

                // ---------- SECURITY: validate and normalize everything ----------

                // 1) Only allow .js uploads
                var fileExtension = Path.GetExtension(jsFile.FileName).ToLowerInvariant();
                var allowedExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".js" };
                if (!allowedExts.Contains(fileExtension))
                {
                    TempData["Message"] = "Only .js files are allowed.";
                    return RedirectToAction("Index", "Home");
                }

                // 2) Sanitize user-controlled component (username) so it is ONE safe path component
                static string SanitizeComponent(string input)
                {
                    if (string.IsNullOrWhiteSpace(input)) return "anon";
                    // allow letters, digits, dot, underscore, hyphen; replace others with underscore
                    var cleaned = Regex.Replace(input, @"[^A-Za-z0-9._-]", "_");
                    cleaned = cleaned.Trim(' ', '.'); // avoid leading/trailing dots or spaces
                    if (cleaned.Length == 0) return "anon";
                    // collapse any accidental parent-dir hints
                    cleaned = cleaned.Replace("..", "_");
                    return cleaned;
                }

                var safeUser = SanitizeComponent(submissionModel.Username);
                var stamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

                // 3) Build names using safe components
                var uniqueFileName = $"game_{safeUser}_{stamp}{fileExtension}";
                var uniqueJsonName = $"meta_{safeUser}_{stamp}.json";
                var folderName = $"{safeUser}_{stamp}";

                // 4) Combine under the approved root, then normalize and verify containment
                static string EnsureUnderRoot(string root, string path)
                {
                    var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                    var fullPath = Path.GetFullPath(path);
                    if (!fullPath.StartsWith(fullRoot, StringComparison.Ordinal))
                        throw new InvalidOperationException("Invalid path.");
                    return fullPath;
                }

                var uniqueFolderCandidate = Path.Combine(submissionDirectory, folderName);
                var uniqueFolderPath = EnsureUnderRoot(submissionDirectory, uniqueFolderCandidate);

                // Now that the path is proven safe, create it.
                Directory.CreateDirectory(uniqueFolderPath);

                // Resolve file targets (and re-verify if desired)
                var filePathCandidate = Path.Combine(uniqueFolderPath, uniqueFileName);
                var jsonPathCandidate = Path.Combine(uniqueFolderPath, uniqueJsonName);

                var filePath = EnsureUnderRoot(submissionDirectory, filePathCandidate);
                var jsonPath = EnsureUnderRoot(submissionDirectory, jsonPathCandidate);

                // ---------- WRITE FILES SAFELY ----------

                // Save the JSON metadata
                await using (var createStream = System.IO.File.Create(jsonPath))
                {
                    await JsonSerializer.SerializeAsync(createStream, new List<GameSubmissionModel> { submissionModel },
                        new JsonSerializerOptions { WriteIndented = true });
                }

                // Save the uploaded JS file
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await jsFile.CopyToAsync(stream);
                }


                TempData["Message"] = "✅ Thank you! Your suggestion has been received (but not stored).";
                //    END OF SAVING ----------------------------------------------------------------------


                TempData["Message"] = "Success! Your game suggestion has been submitted for review.";
                TempData["SubmittedTitle"] = submissionModel.SuggestedTitle;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process game submission.");
                TempData["Message"] = "An unexpected error occurred while processing your submission.";
                return RedirectToAction("Index", "Home");
            }
        }

        private async Task<ValidationResult> ValidateGameSubmissionForm(string title, string author,
            string description, string howToPlay, string thumbnailUrl, IFormFile jsFile)
        {
            var result = new ValidationResult();

            // Create a submission object for validation
            var submissionData = new GameSubmissionJson
            {
                Title = title,
                Author = author,
                Description = description,
                HowToPlay = howToPlay,
                ThumbnailUrl = thumbnailUrl
            };

            // Validate the form fields
            var fieldValidation = ValidateGameSubmission(submissionData);
            if (!fieldValidation.IsValid)
            {
                return fieldValidation;
            }

            // Validate JavaScript file if provided
            if (jsFile != null && jsFile.Length > 0)
            {
                var fileValidation = ValidateJavaScriptFile(jsFile);
                if (!fileValidation.IsValid)
                {
                    return fileValidation;
                }

                // Read and validate JavaScript content
                try
                {
                    using var reader = new StreamReader(jsFile.OpenReadStream());
                    string jsContent = await reader.ReadToEndAsync();

                    if (jsContent.Length > 500000) // 500KB
                    {
                        result.AddError("JavaScriptCode", "JavaScript code is too large (max 500KB).");
                    }

                    if (ContainsDangerousCode(jsContent))
                    {
                        result.AddError("JavaScriptCode", "Code contains potentially dangerous patterns.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading JavaScript file.");
                    result.AddError("JavaScriptFile", "Unable to read JavaScript file content.");
                }
            }

            return result;
        }

        private ValidationResult ValidateJavaScriptFile(IFormFile file)
        {
            var result = new ValidationResult();

            // Check file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (fileExtension != ".js")
            {
                result.AddError("JavaScriptFile", "Only .js files are allowed for code uploads.");
                return result;
            }

            // Check file size (max 500KB)
            const long maxFileSize = 500 * 1024;
            if (file.Length > maxFileSize)
            {
                result.AddError("JavaScriptFile", "JavaScript file must be less than 500KB.");
                return result;
            }

            return result;
        }

        private ValidationResult ValidateGameSubmission(GameSubmissionJson data)
        {
            var result = new ValidationResult();

            // Validate Title
            if (string.IsNullOrWhiteSpace(data.Title))
            {
                result.AddError("Title", "Game title is required.");
            }
            else if (data.Title.Length > 50)
            {
                result.AddError("Title", "Game title must be 50 characters or less.");
            }
            else if (ContainsOffensiveContent(data.Title))
            {
                result.AddError("Title", "Game title contains inappropriate content.");
            }

            // Validate Author
            if (string.IsNullOrWhiteSpace(data.Author))
            {
                result.AddError("Author", "Author name is required.");
            }
            else if (data.Author.Length > 100)
            {
                result.AddError("Author", "Author name must be 100 characters or less.");
            }

            // Validate Description
            if (string.IsNullOrWhiteSpace(data.Description))
            {
                result.AddError("Description", "Game description is required.");
            }
            else if (data.Description.Length < 20)
            {
                result.AddError("Description", "Description must be at least 20 characters.");
            }
            else if (data.Description.Length > 1000)
            {
                result.AddError("Description", "Description must be 1000 characters or less.");
            }

            // Validate How To Play
            if (string.IsNullOrWhiteSpace(data.HowToPlay))
            {
                result.AddError("HowToPlay", "Instructions are required.");
            }
            else if (data.HowToPlay.Length > 1000)
            {
                result.AddError("HowToPlay", "Instructions must be 1000 characters or less.");
            }

            // Validate Thumbnail URL (optional)
            if (!string.IsNullOrWhiteSpace(data.ThumbnailUrl))
            {
                if (!Uri.TryCreate(data.ThumbnailUrl, UriKind.Absolute, out var uriResult)
                    || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    result.AddError("ThumbnailUrl", "Thumbnail URL must be a valid HTTP/HTTPS URL.");
                }
            }

            return result;
        }

        private bool ContainsOffensiveContent(string text)
        {
            // Implement your offensive content filter here
            // This is a placeholder - use a proper profanity filter library in production
            var offensiveWords = new[] { "badword1", "badword2" }; // Replace with actual filter
            return offensiveWords.Any(word => text.Contains(word, StringComparison.OrdinalIgnoreCase));
        }

        private bool ContainsDangerousCode(string code)
        {
            // Basic check for dangerous patterns
            var dangerousPatterns = new[]
            {
                @"eval\s*\(",
                //@"Function\s*\(",
                @"<script",
                @"document\.write",
                @"innerHTML\s*=",
                @"outerHTML\s*=",
                @"\.cookie",
                @"localStorage",
                @"sessionStorage",
                @"XMLHttpRequest",
                @"fetch\s*\(",
                @"import\s*\("
            };

            return dangerousPatterns.Any(pattern =>
                Regex.IsMatch(code, pattern, RegexOptions.IgnoreCase));
        }

        // Helper classes
        public class GameSubmissionJson
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string Description { get; set; }
            public string HowToPlay { get; set; }
            public string ThumbnailUrl { get; set; }
        }

        public class ValidationResult
        {
            public bool IsValid => !Errors.Any();
            public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
            public string ErrorMessage => IsValid ? "" : string.Join(" ", Errors.Values);

            public void AddError(string field, string message)
            {
                Errors[field] = message;
            }
        }
    }
}