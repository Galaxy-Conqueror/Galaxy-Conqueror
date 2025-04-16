using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Models.Responses;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.API.Services
{
    public interface IUserService
    {
        Task<User?> GetUserById(Guid id);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByContext(HttpContext context);
        Task<IEnumerable<User>> GetUsers();
        Task<User> CreateUser(UserInfoResponse userInfo);
        Task<User?> UpdateUser(Guid id, string username);
        Task DeleteUser(Guid id);
    }
}
