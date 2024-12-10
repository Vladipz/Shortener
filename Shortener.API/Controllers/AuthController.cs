using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using Shortener.API.Contracts.Requests;
using Shortener.API.Mappings;
using Shortener.BLL.Exeptions;
using Shortener.BLL.Interfaces;

namespace Shortener.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request.ToCreateModel());

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.ToErrorsList());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (IdentityValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.AuthenticateAsync(request.Email, request.Password);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request.Token, request.RefreshToken);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}