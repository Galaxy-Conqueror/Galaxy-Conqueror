using System.Data;
using System.Security.Claims;
using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Models.Responses;

namespace Galaxy.Conqueror.API.Services;

public class UserService(IDbConnectionFactory connectionFactory, ISetupService setupService) : IUserService
{
    public async Task<User?> GetUserById(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM users WHERE id = @Id";
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM users WHERE email = @Email";
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<User?> GetUserByContext(HttpContext context)
    {
        var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return null;

        return await GetUserByEmail(email);;
    }
   
    public async Task<IEnumerable<User>> GetUsers()
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM users";
        return await connection.QueryAsync<User>(sql);
    }

    public async Task<User?> CreateUser(UserInfoResponse userInfo)
    {
        using var connection = connectionFactory.CreateConnection();

        var googleId = userInfo.sub;
        var email = userInfo.email;
        var username = "";

        const string findUserSql = @"SELECT * FROM users WHERE email = @Email";
        var existingUser = await connection.QuerySingleOrDefaultAsync<User>(findUserSql, new { Email = email });

        if (existingUser != null)
        {
            return existingUser;
        }

        var user = await setupService.SetupPlayerDefaults(email, googleId, username);
        return user;
    }

    public async Task<User?> UpdateUser(Guid id, string username)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE users SET username = @Username 
            WHERE id = @Id
            RETURNING *";
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id, Username = username });
    }

    public async Task DeleteUser(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "DELETE FROM users WHERE id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

}
