namespace Shortener.BLL.Models
{
    public class ShortUrlModel
    {
        public int Id { get; set; }

        public string OriginalUrl { get; set; } = string.Empty;

        public string ShortenedUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public Guid CreatedById { get; set; }

        public string CreatedBy { get; set; } = null!;
    }
}