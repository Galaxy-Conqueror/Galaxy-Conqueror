using System.Data.Common;
using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;
using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Services;

public class PlanetService(IDbConnectionFactory connectionFactory)
{
    public async Task<IEnumerable<Planet>> GetPlanets()
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM planets";
        return await connection.QueryAsync<Planet>(sql);
    }

    public async Task<Planet?> GetPlanetById(Guid id)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM planets WHERE id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Planet>(sql, new { Id = id });
    }

    public async Task<Planet?> GetPlanetByUserID(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM planets WHERE user_id = @UserId";
        return await connection.QuerySingleOrDefaultAsync<Planet>(sql, new { UserId = userId });
    }

    public async Task<Planet> CreatePlanet(Guid userId, DbTransaction? transaction = null)
    {
        using var connection = transaction?.Connection ?? connectionFactory.CreateConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        const string sql = @"
            INSERT INTO planets (user_id)
            VALUES (@UserId)
            RETURNING *;
        ";

        return await connection.QuerySingleAsync<Planet>(
            sql,
            new { UserId = userId },
            transaction: transaction
        );
    }

}
