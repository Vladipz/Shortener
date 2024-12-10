using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

using Shortener.DAL.Interfaces;

namespace Shortener.DAL.Entities
{
    public class ShortenerUser : IdentityUser<Guid>, IDatedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Guid Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        public ICollection<ShortUrl> ShortUrls { get; init; } =
            [];

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}