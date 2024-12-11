using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Shortener.API.Contracts.Requests;
using Shortener.API.Helpers;
using Shortener.BLL.Services;

namespace Shortener.API.Controllers
{
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        private readonly IUrlService _urlShortenerService;

        public ShortUrlController(IUrlService urlShortenerService)
        {
            _urlShortenerService = urlShortenerService;
        }

        [Authorize]
        [HttpPost("/api/ShortUrl/create")] // Повний шлях
        public async Task<IActionResult> CreateShortUrl([FromBody] LongUrlRequest request)
        {
            var userId = HttpContext.GetUserId();
            if (string.IsNullOrWhiteSpace(request.LongUrl))
            {
                return BadRequest("URL cannot be empty.");
            }

            var shortUrl = await _urlShortenerService.CreateShortUrlAsync(request.LongUrl, userId);
            return Ok(shortUrl);
        }

        [HttpGet("ResolveUrl/{shortUrl}")] // Повний шлях
        public async Task<IActionResult> ResolveShortUrl(string shortUrl)
        {
            var longUrl = await _urlShortenerService.GetLongUrlAsync(shortUrl);
            if (longUrl == null)
            {
                return NotFound("Short URL not found.");
            }

            return Redirect(longUrl);
        }

        [HttpGet("api/ShortUrl")]
        public async Task<IActionResult> GetShortUrls()
        {
            var shortUrls = await _urlShortenerService.GetShortUrlsAsync();
            return Ok(shortUrls);
        }
    }
}