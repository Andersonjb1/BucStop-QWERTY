using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BucStop.Models;
using BucStop.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
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
        private readonly SnapshotService _snapshotService;
        private readonly PlayCountManager _playCountManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHostEnvironment _host;

        public AdminController(
            MicroClient microClient,
            ILogger<AdminController> logger,
            SnapshotService snapshotService,
            IWebHostEnvironment webHostEnvironment,
            IHostEnvironment host)
        {
            _snapshotService = snapshotService;
            _httpClient = microClient;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _playCountManager = new PlayCountManager(_httpClient.GetGamesList() ?? new List<Game>(), webHostEnvironment);
            _host = host;
        }

        // Takes the user to the admin page.
        [Authorize]
        public IActionResult Index()
        {
            _logger.LogInformation("{Category}: {User} visited the Admin page.", "UserActivity", User.Identity?.Name ?? "Anonymous");
            return View(_httpClient.GetGamesList());
        }

        // If something goes wrong, this will take the user to a page explaining the error.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
