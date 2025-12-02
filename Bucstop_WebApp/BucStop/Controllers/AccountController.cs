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

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string? returnUrl = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // keep returnUrl available for redisplay if there's an error
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Please enter your email.");

                _logger.LogWarning("{Category}: Empty email entered during login attempt.", "InvalidLogin");

                stopwatch.Stop();
                _logger.LogInformation("{Category}: Empty Email Login Page Loaded in {LoadTime}ms.", "PageLoadTimes", stopwatch.ElapsedMilliseconds);

                return View();
            }

            // ToLower added to remove case sensitivity. Current font makes all lettering look like capital letters.
            if (Regex.IsMatch(email.ToLower(), @"\b[A-Za-z0-9._%+-]+@etsu\.edu\b"))
            {
                // If authentication is successful, create a ClaimsPrincipal and sign in the user
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.NameIdentifier, "user_id"),
                };

                var claimsIdentity = new ClaimsIdentity(claims, "custom");
                var userPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign in the user
                await HttpContext.SignInAsync("CustomAuthenticationScheme", userPrincipal);

                stopwatch.Stop();

                // After successful login
                _logger.LogInformation("{Category}: A user successfully logged in.", "UserActivity");
                _logger.LogInformation("{Category}: Successful Login Page Loaded in {LoadTime}ms.", "PageLoadTimes", stopwatch.ElapsedMilliseconds);

                // if we came from a protected page like /Home/Admin, go back there
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Fallback: same as before
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Authentication failed, return to the login page with an error message
                _logger.LogWarning("{Category}: Invalid Bucstop admin login attempt.", "InvalidLogin");
                ModelState.AddModelError(string.Empty, "Only BucStop admins can login");

                stopwatch.Stop();

                _logger.LogInformation("{Category}: Denied Login Page Loaded in {LoadTime}ms.", "PageLoadTimes", stopwatch.ElapsedMilliseconds);

                return View();
            }
        }

        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            _logger.LogInformation("{Category}: {User} logged out.", "UserActivity", User.Identity?.Name ?? "Anonymous");
            _logger.LogInformation("User logged out.");
            await HttpContext.SignOutAsync("CustomAuthenticationScheme");
            // If we know where they came from (e.g., /Home/Admin), send it to Login
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return RedirectToAction("Login", new { returnUrl });
            }
            // Fallback: normal behavior
            return RedirectToAction("Login");
        }
    }
}
