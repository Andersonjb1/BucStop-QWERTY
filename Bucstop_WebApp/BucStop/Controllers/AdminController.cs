using BucStop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

/*
 * This file has the controllers for everything outside of the games
 * and game-related pages.
 */

namespace BucStop.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly MicroClient _httpClient;

        public AdminController(MicroClient microClient, ILogger<AdminController> logger)
        {
            _logger = logger;
            _httpClient = microClient;
        }
         

        //Takes the user to the admin page.
        [Authorize]
        public IActionResult Index()
        {
            _logger.LogInformation("{Category}: {User} visited the Admin page.", "UserActivity", User.Identity?.Name ?? "Anonymous");
            return View(_httpClient.GetGamesList());
        }

        //If something goes wrong, this will take the user to a page explaining the error.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}