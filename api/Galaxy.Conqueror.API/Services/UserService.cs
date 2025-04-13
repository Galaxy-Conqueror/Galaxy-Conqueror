using System.Data;
using Dapper;
using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Services;

public class UserService(IDbConnection db, SetupService setupService)
{
    private readonly IDbConnection _db = db;

    public async Task<User?> GetUserById(Guid id)
    {
        const string sql = "SELECT * FROM users WHERE id = @Id";
        return await _db.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User> GetUserByEmail(string email)
    {
        const string sql = "SELECT * FROM users WHERE email = @Email";
        return await _db.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<User?> GetUserByContext(HttpContext context)
    {
        // var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
        var email = "user1@example.com";

        if (string.IsNullOrEmpty(email))
            return null;

        return await GetUserByEmail(email);;
    }
   
    public async Task<IEnumerable<User>> GetUsers()
    {
        const string sql = "SELECT * FROM users";
        return await _db.QueryAsync<User>(sql);
    }

    public async Task<User?> CreateUser(UserInfoResponse userInfo)
    {
        var googleId = userInfo.sub;
        var email = userInfo.email;
        var username = "Galaxy Conqueror";

        const string findUserSql = @"SELECT * FROM users WHERE email = @Email";
        var existingUser = await _db.QuerySingleOrDefaultAsync<User>(findUserSql, new { Email = email });

        if (existingUser != null)
        {
            return existingUser;
        }

        var user = await setupService.SetupPlayerDefaults(email, googleId, username);

        return user;
    }

    public async Task<User?> UpdateUser(Guid id, string username)
    {
        const string sql = @"
            UPDATE users SET username = @Username 
            WHERE id = @Id
            RETURNING *";
        return await _db.QuerySingleOrDefaultAsync<User>(sql, new { Id = id, Username = username });
    }

    public async Task DeleteUser(string id)
    {
        const string sql = "DELETE FROM users WHERE id = @Id";
        await _db.ExecuteAsync(sql, new { Id = id });
    }

}
