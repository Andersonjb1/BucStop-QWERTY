using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Pong
{
    [ApiController]
    [Route("[controller]")]
    public class PongController : ControllerBase
    {
        private readonly ILogger<PongController> _logger;
        private readonly IConfiguration _config;
        private static string gameURL;
        private static string imgURL;

        public PongController(ILogger<PongController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            gameURL = _config["MicroserviceUrls:Pong"];
            imgURL = _config["MicroserviceUrls:Image"];
        }

        private static readonly List<GameInfo> TheInfo = new List<GameInfo>
        {
            new GameInfo {
            Id = 3,
            Title = "Pong",
            Content = null, // Will be set dynamically
            Author = "Fall 2023 Semester",
            DateAdded = "",
            Description = "Pong is a classic arcade game where the player uses a paddle to hit a ball against a computer's paddle. Either party scores when the ball makes it past the opponent's paddle.",
            HowTo = "Control with arrow keys.",
            Thumbnail = imgURL
            }
        };

        [HttpGet]
        public async Task<IEnumerable<GameInfo>> Get()
        {
            // Fetch the JavaScript code from the gameURL
            if (string.IsNullOrEmpty(TheInfo[0].Content))
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var jsCode = await httpClient.GetStringAsync(gameURL);
                        TheInfo[0].Content = jsCode;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to fetch JS code from {GameUrl}", gameURL);
                        TheInfo[0].Content = "// Failed to load game code";
                    }
                }
            }

            if (TheInfo[0].Thumbnail == null)
            {
                TheInfo[0].Thumbnail = imgURL;
            }
            return TheInfo;
        }
    }
}