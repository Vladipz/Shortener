using Shortener.BLL.Models;
using Shortener.DAL.Entities;

namespace Shortener.BLL.Mappings
{
    public static class UserMappingExtentions
    {
        public static ShortenerUserModel ToModel(this ShortenerUser user, ICollection<string> roles)
        {
            return new ShortenerUserModel
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles,
            };
        }
    }
}