using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Models.Responses;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.API.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(UserInfoResponse userInfo);
    }
}
