using System.Data.Common;
using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;
using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Services;

public class TurretService(IDbConnectionFactory connectionFactory)
{

    public async Task<Turret> CreateTurret(int planetId, DbTransaction? transaction = null)
    {
        using var connection = transaction?.Connection ?? connectionFactory.CreateConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        const string sql = @"
            INSERT INTO turrets (planet_id, level)
            VALUES (@PlanetId, @Level)
            RETURNING *;
        ";

        return await connection.QuerySingleAsync<Turret>(
            sql,
            new { PlanetId = planetId, Level = 1 },
            transaction: transaction
        );
    }

    public async Task<Turret?> GetTurretById(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM turrets WHERE id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Turret>(sql, new { Id = id });
    }

    public async Task<Turret?> GetTurretByUserID(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = @"
            SELECT turrets.*
            FROM turrets
            JOIN planets p ON turrets.planet_id = p.id
            WHERE p.user_id = @UserId";
        return await connection.QuerySingleOrDefaultAsync<Turret>(sql, new { UserId = userId });
    }

    public async Task<Turret?> GetTurretByPlanetId(int planetId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT t.* FROM planets p JOIN turrets t ON p.id = t.planet_id WHERE t.planet_id = @PlanetId";
        return await connection.QuerySingleOrDefaultAsync<Turret>(sql, new { PlanetId = planetId });
    }

    public async Task<Turret> UpgradeTurret(int turretId, int planetId, int cost)
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

            const string turretSql = @"
                UPDATE turrets
                SET level = level + 1
                WHERE id = @Id
                RETURNING *;
            ";

            var upgradedTurret = await connection.QuerySingleOrDefaultAsync<Turret>(turretSql, new { Id = turretId }, transaction);

            if (upgradedTurret == null)
            {
                throw new Exception("Turret not found or update failed.");
            }

            await transaction.CommitAsync();

            return upgradedTurret;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}