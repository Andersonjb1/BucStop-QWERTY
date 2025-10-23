using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration; // added this
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BucStop.Models;

namespace BucStop.Services
{
    public class ApiHeartbeatService : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiHeartbeatService> _logger;
        private readonly IConfiguration _config;
        private readonly Dictionary<string, string> urlDic;
        private readonly string _gatewayHealthCheckUrl;
        private readonly string _snakeHealthCheckUrl;
        private readonly string _tetrisHealthCheckUrl;
        private readonly string _pongHealthCheckUrl;


        public ApiHeartbeatService(HttpClient httpClient, ILogger<ApiHeartbeatService> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = config;
            var gateway = _config["Gateway"];
            _gatewayHealthCheckUrl = $"{gateway}/health";

            var snake = _config["Snake"];
            _snakeHealthCheckUrl = $"{snake}/health";

            var tetris = _config["Tetris"];
            _tetrisHealthCheckUrl = $"{tetris}/health";

            var pong = _config["Pong"];
            _pongHealthCheckUrl = $"{pong}/health";

            urlDic = new Dictionary<string, string>()
            {
                { "API GateWay", _gatewayHealthCheckUrl },
                { "Snake", _snakeHealthCheckUrl },
                { "Tetris", _tetrisHealthCheckUrl },
                { "Pong", _pongHealthCheckUrl }
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                foreach (var (name, url) in urlDic)
                {

                    try
                    {
                        var response = await _httpClient.GetAsync(url, stoppingToken);

                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("{Category}: {Service} is healthy.", "APIHeartbeat", name);
                        }
                        else
                        {
                            _logger.LogWarning("{Category}: {Service} returned {StatusCode}.", "APIHeartbeat", name, response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("{Category}: {Service} heartbeat failed - {ErrorMessage} url: {URL}", "APIHeartbeat", name, ex.Message, url);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Runs every 5 minutes
            }
        }
    }
}