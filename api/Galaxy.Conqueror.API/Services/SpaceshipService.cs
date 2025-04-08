using System.Data.Common;
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

    public async Task<Spaceship> CreateSpaceship(Guid userId, Planet planet, DbTransaction? transaction = null)
    {
        using var connection = transaction?.Connection ?? connectionFactory.CreateConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        const string sql = @"
            INSERT INTO spaceships (user_id)
            VALUES (@UserId)
            RETURNING *";
        var shapeship = new Spaceship()
        {
            UserId = userId,
            // Design
            // Description
            Level = 1,
            CurrentFuel = 100, // set to max fuel based on level and config values
            CurrentHealth = 400, // set to max health based on level and config values
            ResourceReserve = 0,
            X = planet.X,
            Y = planet.Y + 1,
        };
        return await connection.QuerySingleAsync<Spaceship>(sql, shapeship, transaction: transaction);
    }

    public async Task<Spaceship> UpdateSpaceship(int spaceshipId, int planetId, int cost)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();
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

            await transaction.CommitAsync();

            return upgradedSpaceship;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}