using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Shortener.BLL.Exeptions;
using Shortener.BLL.Interfaces;
using Shortener.BLL.Mappings;
using Shortener.BLL.Models;
using Shortener.DAL.Entities;

namespace Shortener.BLL.Services
{
    public class AuthService : IAuthService
    {
        private static readonly string[] BaseRole =
            ["User"];

        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ShortenerUser> _userManager;
        private readonly IValidator<UserCreateModel> _userCreateValidator;

        public AuthService(
            UserManager<ShortenerUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            IValidator<UserCreateModel> userCreateValidator)
        {
            _userManager = userManager;
            _userCreateValidator = userCreateValidator;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthenticationModel> AuthenticateAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (user.Email == null)
            {
                throw new ArgumentNullException(paramName: nameof(user.Email).ToLower(CultureInfo.CurrentCulture));
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var accessToken = GenerateAccessToken(user.ToModel(userRoles));
            var refreshToken = GenerateRefreshToken();

            var tokens = new AuthenticationModel
            {
                Token = accessToken.Token,
                Expires = accessToken.Expires,
                RefreshToken = refreshToken,
            };

            await UpdateUserRefreshToken(user, tokens.RefreshToken);

            return tokens;
        }

        public async Task<AuthenticationModel> RefreshTokenAsync(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);

            var userId = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var userEmail = principal?.FindFirstValue(JwtRegisteredClaimNames.Email);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            {
                throw new UnauthorizedAccessException("Invalid token: missing user id or email");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var accessToken = GenerateAccessToken(user.ToModel(userRoles));
            var newRefreshToken = GenerateRefreshToken();

            var tokens = new AuthenticationModel
            {
                Token = accessToken.Token,
                Expires = accessToken.Expires,
                RefreshToken = newRefreshToken,
            };

            await UpdateUserRefreshToken(user, tokens.RefreshToken);

            _ = await _userManager.UpdateAsync(user);

            return tokens;
        }

        public async Task<AuthenticationModel> RegisterAsync(UserCreateModel model)
        {
            var validationResult = await _userCreateValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("User with this email already exists");
            }

            var entity = new ShortenerUser
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(entity, model.Password);

            if (!result.Succeeded)
            {
                throw new IdentityValidationException(result.ToValidationErrorsGrouped());
            }

            var createdUser = await _userManager.FindByEmailAsync(model.Email) ?? throw new KeyNotFoundException("User not found");

            var resultRoles = await _userManager.AddToRoleAsync(createdUser, "User");

            if (!resultRoles.Succeeded)
            {
                throw new IdentityValidationException(resultRoles.ToValidationErrorsGrouped());
            }

            var accessToken = GenerateAccessToken(createdUser.ToModel(BaseRole));
            var refreshToken = GenerateRefreshToken();

            return new AuthenticationModel
            {
                Token = accessToken.Token,
                Expires = accessToken.Expires,
                RefreshToken = refreshToken,
            };
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _jwtSettings.ValidIssuer,
                ValidAudience = _jwtSettings.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = false,
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out _);
        }

        private async Task UpdateUserRefreshToken(ShortenerUser user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            _ = await _userManager.UpdateAsync(user);
        }

        private TokenModel GenerateAccessToken(ShortenerUserModel user)
        {
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Email, user.Email),
                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Ensure the JWT secret is set
            if (string.IsNullOrEmpty(_jwtSettings.Secret))
            {
                throw new InvalidOperationException("JWT secret is not set.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationMinutes),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new TokenModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = token.ValidTo,
            };
        }
    }
}