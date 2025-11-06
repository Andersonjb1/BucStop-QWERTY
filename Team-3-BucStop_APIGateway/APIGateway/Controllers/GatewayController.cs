using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Gateway;
using System.ComponentModel.Design;

// TESTING DEPLOY ACTION

namespace Gateway
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly ILogger<GatewayController> _logger;
        private readonly IConfiguration _config;
        private readonly List<GameInfo> _gameInfos;
        private readonly IHostEnvironment _host;

        public GatewayController(ILogger<GatewayController> logger, IConfiguration config, IHostEnvironment host)
        {
            _logger = logger;
            _config = config;
            _host = host;
            _gameInfos = new List<GameInfo>();
        }

        [HttpGet]
        public async Task<IEnumerable<GameInfo>> Get()
        {
            try
            {
                var gameKeys = new[] { "snake", "tetris", "pong" };

                var fetchTasks = new List<Task>();
                foreach (var game in gameKeys)
                {
                    string internalUrl;

                    if (_host.IsDevelopment())
                    {
                        internalUrl = _config[$"Microservices:{game}"];
                        _logger.LogInformation($"Using Development URL for {game}: {internalUrl}");
                    }
                    else
                    {
                        internalUrl = _config[$"Microservices:{game}"];
                        _logger.LogInformation($"Using Container URL for {game}: {internalUrl}");
                    }

                    string publicUrl = _config[$"PublicUrls:{game}"];
                    string jsPath = $"/js/{game.ToLowerInvariant()}.js";

                    fetchTasks.Add(FetchGameInfo(internalUrl, $"/{game}"));
                }

                await Task.WhenAll(fetchTasks);
                return _gameInfos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while gathering game info");
                return GenerateFailureResponse();
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task FetchGameInfo(string internalUrl, string endpoint)
        {
            try
            {
                using var client = new HttpClient { BaseAddress = new Uri(_config[$"Microservices:Gateway"]) };

                _logger.LogInformation($"Using BaseAddress: {client.BaseAddress} and Requesting {internalUrl + endpoint}");

                var response = await client.GetAsync(internalUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var gameInfoList = await response.Content.ReadFromJsonAsync<List<GameInfo>>();

                    _logger.LogInformation($"Game Info: {System.Text.Json.JsonSerializer.Serialize(gameInfoList)}");

                    if (gameInfoList != null)
                    {
                        foreach (var game in gameInfoList)
                        {
                            string gameTitle = game.Title.ToLowerInvariant();
                            string service = $"game-{gameTitle}";

                            game.Content = $"/cache/{service}/js/{gameTitle}.js";
                            game.Thumbnail = $"/cache/{service}/images/{gameTitle}.jpg";
                        }

                        lock (_gameInfos)
                        {
                            _gameInfos.AddRange(gameInfoList);
                        }
                    }
                }
                else
                {
                    _logger.LogError("Failed to get game info from {Url}{Endpoint} - Status: {Status}", internalUrl, endpoint, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in FetchGameInfo for {Url}{Endpoint}", internalUrl, endpoint);
            }
        }

        private IEnumerable<GameInfo> GenerateFailureResponse()
        {
            return new List<GameInfo>
            {
                new GameInfo
                {
                    Title = "Unavailable",
                    Author = "Unavailable",
                    Description = "Failed to retrieve data",
                    HowTo = "N/A",
                    LeaderBoardStack = new Stack<KeyValuePair<string, int>>()
                }
            };
        }
    }
}