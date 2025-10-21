using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Tetris
{
    [ApiController]
    [Route("[controller]")]
    public class TetrisController : ControllerBase
    {
        private readonly ILogger<TetrisController> _logger;
        private readonly IConfiguration _config;
        private static string gameURL;
        private static string imgURL;

        public TetrisController(ILogger<TetrisController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            gameURL = _config["MicroserviceUrls:Tetris"];
            imgURL = _config["MicroserviceUrls:Image"];
        }

        private static readonly List<GameInfo> TheInfo = new List<GameInfo>
        {
            new GameInfo {

                Id = 2,
                Title = "Tetris",
                Author = "Fall 2023 Semester",
                //Content = "~/js/tetris.js", //changing URL now that the MS hosts game code
                //Content = "https://localhost:2626/js/tetris.js",
                Content = gameURL,
                DateAdded = "",
                Description = "Tetris is a classic arcade puzzle game where the player has to arrange falling blocks, also known as Tetronimos, of different shapes and colors to form complete rows on the bottom of the screen. The game gets faster and harder as the player progresses, and ends when the Tetronimos reach the top of the screen.",
                HowTo = "Control with arrow keys: Up arrow to spin, down to speed up fall, space to insta-drop.",
                //Thumbnail = "/images/tetris.jpg"
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