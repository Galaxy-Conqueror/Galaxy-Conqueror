using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;
using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Services;

public class SpaceshipService(IDbConnectionFactory connectionFactory)
{
    public async Task<Spaceship?> GetSpaceshipByUserId(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM spaceships WHERE user_id = @UserId";
        return await connection.QuerySingleOrDefaultAsync<Spaceship>(sql, new { UserId = userId });
    }

    public async Task<Spaceship> UpdateSpaceship(int spaceshipId, int planetId, int cost)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            const string updatePlanetSql = @"
                UPDATE planets
                SET resource_reserve = resource_reserve - @Cost
                WHERE id = @Id
            ";

            var affectedRows = await connection.ExecuteAsync(updatePlanetSql, new { Id = planetId, Cost = cost }, transaction);

            if (affectedRows == 0)
            {
                throw new Exception("Insufficient resources or planet not found.");
            }

            const string updateSpaceshipSql = @"
                UPDATE spaceships
                SET level = level + 1
                WHERE id = @Id
                RETURNING *;
            ";

            var upgradedSpaceship = await connection.QuerySingleOrDefaultAsync<Spaceship>(updateSpaceshipSql, new { Id = spaceshipId }, transaction);

            if (upgradedSpaceship == null)
            {
                throw new Exception("Spaceship not found or update failed.");
            }

            transaction.Commit();

            return upgradedSpaceship;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

}