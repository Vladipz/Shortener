namespace Shortener.BLL.Models
{
    public class TokenModel
    {
        public string Token { get; set; } = default!;

        public DateTime Expires { get; set; } = default!;
    }
}