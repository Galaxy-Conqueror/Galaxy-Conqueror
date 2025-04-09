using System.Data.Common;
using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Utils;

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
            CurrentFuel = Calculations.GetSpaceshipMaxFuel(1), // set to max fuel based on level and config values
            CurrentHealth = Calculations.GetSpaceshipMaxHealth(1), // set to max health based on level and config values
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

    public async Task<Spaceship> ResetSpaceship(int spaceshipId, Planet planet, DbTransaction? transaction = null)
    {
        using var connection = transaction?.Connection ?? connectionFactory.CreateConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();


            const string resetSpaceshipSql = @"
                UPDATE spaceships SET
                    Level = 1,
                    current_fuel = 0,
                    current_health = 1,
                    resource_reserve = 0,
                    X = @PlanetX,
                    Y = @PlanetY WHERE spaceships.Id = @Id
                    RETURNING *;
            ";

            var resetSpaceship = await connection.QuerySingleOrDefaultAsync<Spaceship>(resetSpaceshipSql, new { Id = spaceshipId, PlanetX = planet.X, PlanetY = planet.Y }, transaction);

            if (resetSpaceship == null)
            {
                throw new Exception("Spaceship reset failed.");
            }

            return resetSpaceship;

    }

    public async Task<Spaceship> LootResources(int spaceshipId, int planetId, int resourcesLooted, DbTransaction? transaction = null)
    {
        using var connection = transaction?.Connection ?? connectionFactory.CreateConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

            const string lootFromPlanetSql = @"
                UPDATE planets
                SET resource_reserve = resource_reserve - @ResourcesLooted
                WHERE id = @Id
                RETURNING *;
            ";

            var affectedRows = await connection.ExecuteAsync(lootFromPlanetSql, new { Id = planetId, ResourcesLooted = resourcesLooted }, transaction);

            if (affectedRows == 0)
            {
                throw new Exception("Insufficient resources or planet not found.");
            }

            const string lootToSpaceshipSql = @"
                UPDATE spaceships
                SET resource_reserve = resource_reserve + @ResourcesLooted
                WHERE id = @Id
                RETURNING *;
            ";

            var spaceshipWithLoot = await connection.QuerySingleOrDefaultAsync<Spaceship>(lootToSpaceshipSql, new { Id = spaceshipId, ResourcesLooted = resourcesLooted }, transaction: transaction);

            if (spaceshipWithLoot == null)
            {
                throw new Exception("Spaceship not found or update failed.");
            }

            return spaceshipWithLoot;

    }
    public async Task<Spaceship> UpdateSpaceshipHealth(int spaceshipId, int damageToSpaceship, DbTransaction? transaction = null)
    {
        using var connection = transaction?.Connection ?? connectionFactory.CreateConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

            const string updateSpaceshipSql = @"
                UPDATE spaceships
                SET current_health = current_health - @DamageToSpaceship
                WHERE id = @Id
                RETURNING *;
            ";

            var updatedSpaceship = await connection.QuerySingleOrDefaultAsync<Spaceship>(updateSpaceshipSql, new { Id = spaceshipId, DamageToSpaceship = damageToSpaceship }, transaction: transaction);

            if (updatedSpaceship == null)
            {
                throw new Exception("Spaceship not found or update failed.");
            }

            return updatedSpaceship;

    }

}