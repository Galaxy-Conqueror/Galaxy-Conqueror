using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Services
{
    public interface ISetupService
    {
        Task<User> SetupPlayerDefaults(string email, string googleId, string username);
    }
}