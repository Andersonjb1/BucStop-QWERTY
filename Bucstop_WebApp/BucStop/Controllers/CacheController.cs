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

  // API Controller that provides an in-memory cache for static assets
  // (JavaScript and images) coming from internal game microservices.
  //
  // Purpose:
  // - On first request for an asset (e.g. /cache/game-snake/js/snake.js) the controller
  //   will fetch the byte content from the internal service via IHttpClientFactory
  //   (using Docker service DNS such as http://game-snake/), store the bytes in IMemoryCache,
  //   and return the content to the browser with appropriate Content-Type.
  // - Subsequent requests for the same asset are served from memory until the cache entry expires.
  //
  // Important:
  // - Cache is in-memory and EPHEMERAL. Cache entries are lost on process/container restart.
  // - The controller performs normalization/whitelisting to avoid becoming an open proxy.
  // - The route prefix is "cache/" making the full route: /cache/{service}/{**path}
  //
  // Security note:
  // - This controller should run in the host that is the browser-facing entry point (webapp),
  //   and must share the Docker network with the game services so it can reach http://{service}/.

  [ApiController]
  [Route("cache/")]
  public class CacheController : Controller
  {
    // used to call internal services
    private readonly IHttpClientFactory _httpFactory;
    // stores fetched bytes keyed by "service:path"
    private readonly IMemoryCache _cache;


    // **Hardcoded for now**
    // Hardcoded allowlist of recognized internal game services.
    // Only requests for these services will be proxied/fetched.
    private static readonly HashSet<string> AllowedServices = new() { "snake", "pong", "tetris" };

    // Constructor - dependencies injected by DI
    public CacheController(IHttpClientFactory httpFactory, IMemoryCache cache)
    {
      _httpFactory = httpFactory;
      _cache = cache;
    }

    // Example: GET /cache/game-snake/js/snake.js
    //
    // Path parameters:
    // - service: the service identifier; accepts both "snake" and "game-snake" (normalized below)
    // - path: the remainder of the asset path within the service, e.g. "js/snake.js" or "images/snake.jpg"
    //
    // Behavior:
    // 1. Validate inputs and normalize the service name (strip "game-" prefix).
    // 2. Verify the normalized service is in the AllowedServices set.
    // 3. Check IMemoryCache for an existing byte[] for the key "<service>:<path>".
    //    - If found: return bytes 
    // 4. If not found: create an HttpClient, set BaseAddress to "http://{service}/", and GET the path.
    //    - On success: read bytes, store in cache, and return bytes to caller.
    //    - On failure: return appropriate HTTP status (502 for network errors or upstream status code).
    //
    // Caching policy:
    // - AbsoluteExpirationRelativeToNow = 30 minutes
    // - SlidingExpiration = 10 minutes
    // Adjust these values to control memory usage.
    //
    // Content-Type detection:
    // - Uses FileExtensionContentTypeProvider to map file extensions to MIME types.
    // - Falls back to application/octet-stream if unknown.
    //
    // Note: The downside to this approach is because data is stored in memory,
    //       large assets will consume process memory; limits need to be acceptable.
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

      // Try to serve from in-memory cache first
      if (_cache.TryGetValue(cacheKey, out byte[] cachedBytes))
      {
        var ctProvider = new FileExtensionContentTypeProvider();
        if (!ctProvider.TryGetContentType(path, out var contentType)) contentType = "application/octet-stream";
        // Public cache control header instructs browser/CDN caching; this can be tuned as needed
        Response.Headers["Cache-Control"] = "public, max-age=3600";
        return File(cachedBytes, contentType);
      }

      // Not in cache: fetch from internal microservice via Docker internal DNS
      var client = _httpFactory.CreateClient();

      // BaseAddress targets the internal service name; rely on Docker network DNS resolution
      client.BaseAddress = new Uri($"http://{normalizedService}/");

      HttpResponseMessage resp;
      try
      {
        resp = await client.GetAsync(path);
      }
      catch
      {
        // Upstream service unreachable or DNS failure -> Bad Gateway
        return StatusCode(502);
      }

      if (!resp.IsSuccessStatusCode) return StatusCode((int)resp.StatusCode);

      var bytes = await resp.Content.ReadAsByteArrayAsync();

      // Cache the bytes in memory with a modest TTL
      var cacheEntryOptions = new MemoryCacheEntryOptions
      {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
        SlidingExpiration = TimeSpan.FromMinutes(10)
      };

      _cache.Set(cacheKey, bytes, cacheEntryOptions);

      // Determine content type and return file content
      var provider = new FileExtensionContentTypeProvider();
      if (!provider.TryGetContentType(path, out var ct)) ct = "application/octet-stream";
      Response.Headers["Cache-Control"] = "public, max-age=3600";
      return File(bytes, ct);
    }
  }
}