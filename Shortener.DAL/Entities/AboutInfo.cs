namespace Shortener.DAL.Entities
{
    public class AboutInfo
    {
        public int Id { get; set; }

        public string Description { get; set; } = string.Empty;

        // Властивість для відстеження, хто останнім редагував
        // TODO: Change way of tracking last update
        public Guid UpdatedById { get; set; }

        public ShortenerUser? UpdatedBy { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}