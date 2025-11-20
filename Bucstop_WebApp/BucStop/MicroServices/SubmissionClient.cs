using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;
using BucStop.Models;
using BucStop.Controllers;

namespace BucStop
{
  public class SubmissionClient
  {

    private readonly JsonSerializerOptions options = new JsonSerializerOptions()
    {
      PropertyNameCaseInsensitive = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly HttpClient submissionClient;
    private readonly ILogger<SubmissionClient> _logger;

    private List<Game> submissionsList;
    private Task<List<Game>> submissionsTask;

    public SubmissionClient(HttpClient submissionClient, ILogger<SubmissionClient> logger)
    {
      this.submissionClient = submissionClient;
      this._logger = logger;

      //Start Asynchronous pull of Submissions
      submissionsTask = GetSubmissionsWithInfo();
    }

    /// <summary>
    /// Requests the SubmissionGateway for a List of Submission Information 
    /// </summary>
    /// <returns></returns>

    public async Task<GameInfo[]> GetSubmissionsAsync()
    {
      try
      {
        var responseMessage = await submissionClient.GetAsync("/SubmissionGateway");

        if (responseMessage != null)
        {
          var stream = await responseMessage.Content.ReadAsStreamAsync();
          return await JsonSerializer.DeserializeAsync<GameInfo[]>(stream, options);
        }
      }
      catch (HttpRequestException ex)
      {
        _logger.LogError("{Category}: API request failed: {ErrorMessage}", "APIRequests", ex.Message);
      }
      return new List<GameInfo>().ToArray();
    }

    public async Task<List<Game>> GetSubmissionsWithInfo()
    {
      List<Game> games = new List<Game>();

      try
      {
        GameInfo[] gameInfos = await GetSubmissionsAsync();

        if (gameInfos.Length > 0)
        {
          _logger.LogInformation("Successfully retrieved {Count} games from API.", gameInfos.Length);
        }
        else
        {
          _logger.LogWarning("API returned 0 games.");
        }

        foreach (GameInfo info in gameInfos)
        {
            if (info == null)
                continue;

            Game game = new Game
            {
                Id = info.Id,
                Title = info.Title,
                Content = info.Content,
                Thumbnail = info.Thumbnail,
                Author = info.Author,
                HowTo = info.HowTo,
                DateAdded = info.DateAdded,
                Description = $"{info.Description} \n {info.DateAdded}",
                LeaderBoard = info.LeaderBoard,
                FolderName = info.FolderName // <-- IMPORTANT
            };

            _logger.LogInformation("Game ID {Id} Content URL: {Content}", info.Id, info.Content);

            games.Add(game);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving game information from API.");
      }

      return games;
    }

    // Return the private submissionsList object.

    public List<Game> GetSubmissionsList()
    {
      submissionsList = submissionsTask.Result;
      return this.submissionsList;
    }
  }
}