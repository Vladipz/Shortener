using Shortener.BLL.Models;

namespace Shortener.BLL.Services
{
    public interface IUrlService
    {
        // Генерація короткого URL за допомогою Base62
        string GenerateShortUrl(int id);

        // Декодування короткого URL назад у ID
        int DecodeShortUrl(string shortUrl);

        // Створення короткого URL і збереження його в базі
        Task<ShortUrlModel> CreateShortUrlAsync(string longUrl, Guid createdById);

        // Перетворення короткого URL на оригінальний
        Task<string> GetLongUrlAsync(string shortUrl);

        // Перевірка чи існує короткий URL в базі даних
        Task<bool> ShortUrlExistsAsync(string shortUrl);

        Task<IEnumerable<ShortUrlModel>> GetShortUrlsAsync();
    }
}