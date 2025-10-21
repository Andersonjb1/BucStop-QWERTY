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
                //Content = "~/js/pong.js",
                //Content = "https://localhost:1941/js/pong.js",
                Content = gameURL,
                Author = "Fall 2023 Semester",
                DateAdded = "",
                Description = "Pong is a classic arcade game where the player uses a paddle to hit a ball against a computer's paddle. Either party scores when the ball makes it past the opponent's paddle.",
                HowTo = "Control with arrow keys.",
                //Thumbnail = "/images/pong.jpg"
                Thumbnail = imgURL
            }

        };

        [HttpGet]
        public IEnumerable<GameInfo> Get()
        {
            // Confirm the Content and Thumbnail URLs are assigned if they were not available when initialized
            if (TheInfo[0].Content == null)
            {
                TheInfo[0].Content = gameURL;
            }

            if (TheInfo[0].Thumbnail == null)
            {
                TheInfo[0].Thumbnail = imgURL;
            }
            return TheInfo;
        }
    }
}