using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;

namespace BucStop.Controllers
{
  [ApiController]
  [Route("cache/")]
  public class AssetsCacheController : Controller
  {
    private readonly IHttpClientFactory _httpFactory;
    private readonly IMemoryCache _cache;


    // Hardcoded for now
    private static readonly HashSet<string> AllowedServices = new() { "snake", "pong", "tetris" };

    public AssetsCacheController(IHttpClientFactory httpFactory, IMemoryCache cache)
    {
      _httpFactory = httpFactory;
      _cache = cache;
    }

    // Example: GET /cache/game-snake/js/snake.js
    [HttpGet("{service}/{**path}")]
    public async Task<IActionResult> GetCached(string service, string path)
    {
      if (string.IsNullOrWhiteSpace(service) || string.IsNullOrWhiteSpace(path))
        return BadRequest("Invalid service or path.");

      // normalize service name: allow "game-snake" or "snake"
      var normalizedService = service.StartsWith("game-") ? service.Substring(5) : service;

      if (!AllowedServices.Contains(normalizedService))
        return BadRequest("Invalid service or path.");

      var cacheKey = $"{normalizedService}:{path}";

      if (_cache.TryGetValue(cacheKey, out byte[] cachedBytes))
      {
        var ctProvider = new FileExtensionContentTypeProvider();
        if (!ctProvider.TryGetContentType(path, out var contentType)) contentType = "application/octet-stream";
        Response.Headers["Cache-Control"] = "public, max-age=3600";
        return File(cachedBytes, contentType);
      }

      var client = _httpFactory.CreateClient();
      client.BaseAddress = new Uri($"http://{normalizedService}/");

      HttpResponseMessage resp;
      try
      {
        resp = await client.GetAsync(path);
      }
      catch
      {
        return StatusCode(502);
      }

      if (!resp.IsSuccessStatusCode) return StatusCode((int)resp.StatusCode);

      var bytes = await resp.Content.ReadAsByteArrayAsync();

      var cacheEntryOptions = new MemoryCacheEntryOptions
      {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
        SlidingExpiration = TimeSpan.FromMinutes(10)
      };

      _cache.Set(cacheKey, bytes, cacheEntryOptions);

      var provider = new FileExtensionContentTypeProvider();
      if (!provider.TryGetContentType(path, out var ct)) ct = "application/octet-stream";
      Response.Headers["Cache-Control"] = "public, max-age=3600";
      return File(bytes, ct);
    }
  }
}