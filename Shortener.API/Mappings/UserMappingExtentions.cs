using Shortener.API.Contracts.Requests;
using Shortener.BLL.Models;

namespace Shortener.API.Mappings
{
    public static class UserMappingExtentions
    {
        public static UserCreateModel ToCreateModel(this RegistrationRequest request)
        {
            return new UserCreateModel
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password,
            };
        }
    }
}