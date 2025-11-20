using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gateway
{
    [ApiController]
    [Route("[controller]")]

    // When using volumes we'll need both volumes accessible in the docker-compose file for Webservice and SubmissionAPIGateway.
    // BUT since this Webservice is only writing to the data, and SubmissionAPIGateway is only reading from the data, they're still indepedent microservices I believe.
    public class SubmissionGatewayController : ControllerBase
    {
        private readonly ILogger<SubmissionGatewayController> _logger;
        private readonly string _submissionDirectory = "/app/Submissions";

        public SubmissionGatewayController(ILogger<SubmissionGatewayController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<GameInfo>> Get()
        {
            var gameInfos = new List<GameInfo>();

            try
            {
                if (!Directory.Exists(_submissionDirectory))
                {
                    _logger.LogWarning("Submission directory not found at {dir}", _submissionDirectory);
                    return gameInfos;
                }

                var submissionFolders = Directory.GetDirectories(_submissionDirectory);
                _logger.LogInformation("Found {Count} submission folder(s)", submissionFolders.Length);

                int idCounter = 1;

                foreach (var folder in submissionFolders)
                {
                    try
                    {
                        _logger.LogInformation("Processing folder: {Folder}", folder);
                        // extract folder name
                        var folderName = Path.GetFileName(folder);

                        // Below finds the json files
                        var jsonFiles = Directory.GetFiles(folder, "*.json");
                        if (jsonFiles.Length == 0)
                        {
                            _logger.LogWarning("No JSON found in folder {folder}", folder);
                            continue;
                        }
                        _logger.LogInformation("Found {Count} JSON file(s) in folder {Folder}", jsonFiles.Length, folder);

                        // Below finds the javascript file
                        var jsFiles = Directory.GetFiles(folder, "*.js");
                        var jsRelativePath = jsFiles.Length > 0
                            ? $"/Submissions/{Path.GetFileName(folder)}/{Path.GetFileName(jsFiles[0])}"
                            : string.Empty;
                        if (string.IsNullOrEmpty(jsRelativePath))
                            _logger.LogWarning("No JS file found in folder {Folder}", folder);

                        foreach (var jsonFile in jsonFiles)
                        {
                            _logger.LogInformation("Reading JSON file: {File}", jsonFile);
                            await using var stream = System.IO.File.OpenRead(jsonFile);

                            List<GameSubmissionModel>? submissions;
                            try
                            {
                                submissions = await JsonSerializer.DeserializeAsync<List<GameSubmissionModel>>(stream);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to deserialize JSON file {File}", jsonFile);
                                continue;
                            }

                            if (submissions == null || submissions.Count == 0)
                            {
                                _logger.LogWarning("No submissions found in JSON file {File}", jsonFile);
                                continue;
                            }

                            _logger.LogInformation("Found {Count} submission(s) in JSON file {File}", submissions.Count, jsonFile);

                            // Convert each submission to GameInfo
                            foreach (var sub in submissions)
                            {
                                var game = new GameInfo
                                {
                                    Id = idCounter++,
                                    Title = sub.SuggestedTitle ?? "Untitled",
                                    Author = sub.SuggestedAuthor ?? "Unknown",
                                    Description = sub.SuggestedDescription ?? "",
                                    HowTo = sub.SuggestedHowTo ?? "",
                                    DateAdded = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                                    Thumbnail = sub.SuggestedThumbnailUrl ?? "",
                                    Content = jsRelativePath,
                                    LeaderBoardStack = GenerateDummyLeaderboard(),

                                    FolderName = folderName
                                };

                                _logger.LogInformation("Added game: {Title} by {Author}", game.Title, game.Author);
                                gameInfos.Add(game);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing folder {Folder}", folder);
                    }
                }

                _logger.LogInformation("Total games loaded: {Count}", gameInfos.Count);
                return gameInfos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading submissions");
                return gameInfos;
            }
        }

        // Remove later on, fills leaderboard with dummy data for the time being to figure out later.
        private static Stack<KeyValuePair<string, int>> GenerateDummyLeaderboard()
        {
            return new Stack<KeyValuePair<string, int>>(new[]
            {
                new KeyValuePair<string, int>("Player1", 100),
                new KeyValuePair<string, int>("Player2", 90),
                new KeyValuePair<string, int>("Player3", 75)
            });
        }
    }

    public class GameSubmissionModel
    {
        public string Username { get; set; } = string.Empty;
        public string SuggestedTitle { get; set; } = string.Empty;
        public string SuggestedAuthor { get; set; } = string.Empty;
        public string SuggestedDescription { get; set; } = string.Empty;
        public string SuggestedHowTo { get; set; } = string.Empty;
        public string SuggestedThumbnailUrl { get; set; } = string.Empty;
        public string RawJsCodeContent { get; set; } = string.Empty;
    }
}
