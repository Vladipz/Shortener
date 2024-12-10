using System.ComponentModel.DataAnnotations;

namespace Shortener.API.Contracts.Requests
{
    public class RegistrationRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}