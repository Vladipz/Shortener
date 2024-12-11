using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shortener.BLL.Mappings;
using Shortener.BLL.Models;
using Shortener.DAL.Data;
using Shortener.DAL.Entities;

namespace Shortener.BLL.Services
{
    public class UrlService : IUrlService
    {
        private const string Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private readonly AppSettings _appSettings;
        private readonly ShortenerDbContext _dbContext;

        public UrlService(IOptions<AppSettings> appSettings, ShortenerDbContext dbContext)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
        }

        public string GenerateShortUrl(int id)
        {
            if (id < 0)
            {
                throw new ArgumentException("ID must be a non-negative number", nameof(id));
            }

            if (id == 0)
            {
                return Chars[0].ToString();
            }

            var shortUrl = new StringBuilder();

            while (id > 0)
            {
                shortUrl.Insert(0, Chars[id % 62]);
                id /= 62;
            }

            return shortUrl.ToString();
        }

        public int DecodeShortUrl(string shortUrl)
        {
            if (string.IsNullOrEmpty(shortUrl))
            {
                throw new ArgumentException("Short URL cannot be null or empty", nameof(shortUrl));
            }

            int id = 0;
            foreach (char c in shortUrl)
            {
                int charIndex = Chars.IndexOf(c);
                if (charIndex == -1)
                {
                    throw new ArgumentException($"Invalid character in short URL: {c}", nameof(shortUrl));
                }

                // Prevent potential overflow by checking before multiplication
                checked
                {
                    id = id * 62 + charIndex;
                }
            }

            return id;
        }

        // Створення короткого URL і збереження його в базі
        public async Task<ShortUrlModel> CreateShortUrlAsync(string longUrl, Guid createdById)
        {
            var urlRecord = CreateUrlRecord(longUrl, createdById);

            var entity = await AddUrlToDatabaseAsync(urlRecord);

            entity.ShortenedUrl = GenerateShortUrl(entity.Id);

            await UpdateUrlWithShortenedUrlAsync(entity);

            var shortUrlWithUser = await GetUrlWithUserAsync(entity.Id);

            shortUrlWithUser.ShortenedUrl = BuildFullShortUrl(shortUrlWithUser.ShortenedUrl);

            return shortUrlWithUser.ToModel();
        }

        private ShortUrlModel CreateUrlRecord(string longUrl, Guid createdById)
        {
            return new ShortUrlModel
            {
                OriginalUrl = longUrl,
                CreatedById = createdById,
                CreatedAt = DateTime.UtcNow,
            };
        }

        private async Task<ShortUrl> AddUrlToDatabaseAsync(ShortUrlModel urlRecord)
        {
            var entity = urlRecord.ToEntity();
            _dbContext.ShortUrls.Add(entity);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"Entity ID: {entity.Id}");
            return entity;
        }

        private async Task UpdateUrlWithShortenedUrlAsync(ShortUrl entity)
        {
            _dbContext.ShortUrls.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<ShortUrl> GetUrlWithUserAsync(int entityId)
        {
            return await _dbContext.ShortUrls
                .Include(s => s.CreatedBy)
                .FirstOrDefaultAsync(s => s.Id == entityId);
        }

        private string BuildFullShortUrl(string shortenedUrl)
        {
            return $"{_appSettings.ShortenUrlDomain}/{_appSettings.ShortUrlPath}/{shortenedUrl}";
        }

        // Перетворення короткого URL на оригінальний (повний URL з доменом і шляхом)
        public async Task<string> GetLongUrlAsync(string shortUrl)
        {
            var id = DecodeShortUrl(shortUrl); // Перетворення короткого URL в ID
            var urlRecord = await _dbContext.ShortUrls
                .FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new KeyNotFoundException("Short URL not found");

            return urlRecord.OriginalUrl; // Повертаємо оригінальний URL
        }

        public async Task<IEnumerable<ShortUrlModel>> GetShortUrlsAsync()
        {
            var urls = await _dbContext.ShortUrls
                .Include(s => s.CreatedBy)
                .ToListAsync(); // Отримуємо список всіх коротких URL з бази даних

            // Формуємо повний URL для кожного запису
            var fullUrls = urls.Select(s => new ShortUrlModel
            {
                Id = s.Id,
                OriginalUrl = s.OriginalUrl,
                ShortenedUrl = $"{_appSettings.ShortenUrlDomain}/{_appSettings.ShortUrlPath}/{s.ShortenedUrl}", // Формуємо повний URL
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy.UserName,
            }).ToList();

            return fullUrls; // Повертаємо оновлений список з повними URL
        }

        // Перевірка чи існує короткий URL в базі даних
        public async Task<bool> ShortUrlExistsAsync(string shortUrl)
        {
            var id = DecodeShortUrl(shortUrl);
            return await _dbContext.ShortUrls
                .AnyAsync(u => u.Id == id);
        }
    }
}