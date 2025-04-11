using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;
using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Services;

public class ResourceExtractorService(IDbConnectionFactory connectionFactory)
{
    // extension
    public async Task<ResourceExtractor?> GetResourceExtractorById(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM resource_extractors WHERE id = @Id";
        return await connection.QuerySingleOrDefaultAsync<ResourceExtractor>(sql, new { Id = id });
    }

    public async Task<ResourceExtractor?> GetResourceExtractorByUserID(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = @"
            SELECT resource_extractors.*
            FROM resource_extractors
            JOIN planets p ON resource_extractors.planet_id = p.id
            WHERE p.user_id = @UserId";
        return await connection.QuerySingleOrDefaultAsync<ResourceExtractor>(sql, new { UserId = userId });
    }

    public async Task<ResourceExtractor> UpgradeResourceExtractor(int resourceExtractorId, int planetId, int cost)
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

            const string resourceExtractorSql = @"
                UPDATE resource_extractors
                SET level = level + 1
                WHERE id = @Id
                RETURNING *;
            ";

            var upgradedResourceExtractor = await connection.QuerySingleOrDefaultAsync<ResourceExtractor>(resourceExtractorSql, new { Id = resourceExtractorId }, transaction);

            if (upgradedResourceExtractor == null)
            {
                throw new Exception("Resource extractor not found or update failed.");
            }

            await transaction.CommitAsync();

            return upgradedResourceExtractor;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}