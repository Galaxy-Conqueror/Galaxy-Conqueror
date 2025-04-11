using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;

namespace Galaxy.Conqueror.API.Services;

public class ResourceUpdaterService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly TimeSpan interval = TimeSpan.FromMinutes(2);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await UpdatePlanetResources();
            await Task.Delay(interval, ct);
        }
    }

    private async Task UpdatePlanetResources()
    {
        using var scope = scopeFactory.CreateScope();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using var connection = connectionFactory.CreateConnection();

        const string query = @"
            SELECT re.id AS ExtractorId, re.level, re.planet_id
            FROM resource_extractors re
        ";

        var extractors = await connection.QueryAsync<(int ExtractorId, int Level, int PlanetId)>(query);

        var resourceUpdates = extractors
            .Select(e => new
            {
                e.PlanetId,
                Amount = e.Level * 10 // TODO Fix rate
            })
            .ToList();

        foreach (var update in resourceUpdates)
        {
            const string updateSql = @"
                UPDATE planets
                SET resource_reserve = resource_reserve + @Amount
                WHERE id = @PlanetId
            ";
            await connection.ExecuteAsync(updateSql, update);
        }
    }
}
