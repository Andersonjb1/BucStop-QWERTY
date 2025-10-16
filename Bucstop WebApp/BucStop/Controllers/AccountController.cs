using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BucStop.Controllers
{
    public class AccountController : Controller
    {
        public string email { get; set; } = string.Empty;

        private readonly ILogger<AccountController> _logger;

        // Following method is to solve a security issue, but CIDR blocks should work so we shouldn't even need emails in the next sprint or 2
        // Helper method to mask email addresses for logs
        private string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "";
            var atIdx = email.IndexOf('@');
            if (atIdx <= 1)
                return "***" + email.Substring(atIdx);
            // show first letter, mask rest before @
            return email.Substring(0, 1) + "***" + email.Substring(atIdx);
        }

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Please enter your email.");

                _logger.LogWarning("{Category}: Empty email entered during login attempt.", "InvalidLogin");

                stopwatch.Stop();
                _logger.LogInformation("{Category}: Empty Email Login Page Loaded in {LoadTime}ms.", "PageLoadTimes", stopwatch.ElapsedMilliseconds);

                return View();
            }

            string maskedEmail = MaskEmail(email);
            //ToLower added to remove case sensitivity. Current Font makes all lettering look like capital letters.
            if (Regex.IsMatch(email.ToLower(), @"\b[A-Za-z0-9._%+-]+@etsu\.edu\b"))
            {
                string localMaskedEmail = MaskEmail(email);
                // If authentication is successful, create a ClaimsPrincipal and sign in the user
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, localMaskedEmail),
                    new Claim(ClaimTypes.NameIdentifier, "user_id"),
                };

                var claimsIdentity = new ClaimsIdentity(claims, "custom");
                var userPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign in the user
                await HttpContext.SignInAsync("CustomAuthenticationScheme", userPrincipal);

                stopwatch.Stop();

                _logger.LogInformation("{Category}: {MaskedEmail} successfully logged in.", "UserActivity", localMaskedEmail);
                _logger.LogInformation("{Category}: Successful Login Page Loaded in {LoadTime}ms.", "PageLoadTimes", stopwatch.ElapsedMilliseconds);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Authentication failed, return to the login page with an error message
                _logger.LogWarning("{Category}: Invalid login attempt for {Email}", "InvalidLogin", maskedEmail);
                ModelState.AddModelError(string.Empty, "Only ETSU students can play, sorry :(");

                stopwatch.Stop();

                _logger.LogInformation("{Category}: Denied Login Page Loaded in {LoadTime}ms.", "PageLoadTimes", stopwatch.ElapsedMilliseconds);

                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("{Category}: {User} logged out.", "UserActivity", User.Identity?.Name ?? "Anonymous");

            _logger.LogInformation("User logged out.");
            await HttpContext.SignOutAsync("CustomAuthenticationScheme");
            return RedirectToAction("Login");
        }


    }
}