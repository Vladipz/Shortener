using Shortener.BLL.Models;

namespace Shortener.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthenticationModel> AuthenticateAsync(string email, string password);

        Task<AuthenticationModel> RefreshTokenAsync(string token, string refreshToken);

        Task<AuthenticationModel> RegisterAsync(UserCreateModel model);

        // Task<bool> RevokeTokenAsync(string token);
    }
}