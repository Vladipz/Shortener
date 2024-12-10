namespace Shortener.BLL.Models
{
    public class JwtSettings
    {
        public string Secret { get; set; } = default!;

        public string ValidIssuer { get; set; } = default!;

        public string ValidAudience { get; set; } = default!;

        public int TokenExpirationMinutes { get; set; } = default!;
    }
}