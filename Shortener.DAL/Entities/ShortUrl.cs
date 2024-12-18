namespace Shortener.DAL.Entities
{
    public class ShortUrl
    {
        public int Id { get; set; }

        public string OriginalUrl { get; set; } = string.Empty;

        public string? ShortenedUrl { get; set; } = null;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public Guid CreatedById { get; set; }

        public ShortenerUser CreatedBy { get; set; } = null!;
    }
}