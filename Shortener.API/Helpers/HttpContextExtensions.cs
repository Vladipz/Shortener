using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shortener.API.Helpers
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            // Перевіряємо, чи userId не є null
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in the token.");
            }

            // Використовуємо TryParse для безпечного перетворення рядка в Guid
            if (Guid.TryParse(userId, out Guid result))
            {
                return result;
            }
            else
            {
                throw new FormatException("Invalid user ID format in the token.");
            }
        }

        public static IEnumerable<string> GetUserRoles(this HttpContext httpContext)
        {
            return httpContext.User.FindAll(ClaimTypes.Role).Select(claim => claim.Value);
        }
    }
}