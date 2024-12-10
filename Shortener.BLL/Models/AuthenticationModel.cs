namespace Shortener.BLL.Models
{
    public class AuthenticationModel
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expires { get; set; }

        public string RefreshToken { get; set; } = string.Empty;
    }
}