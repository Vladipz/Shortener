namespace Shortener.BLL.Models
{
    public class ShortenerUserModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public ICollection<string> Roles { get; init; } =
            [];
    }
}