using System;
using Dapper;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Services;

namespace Galaxy.Conqueror.Tests.Integration.Services;

public class TurretServiceIntegrationTests : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    private readonly PostgreSqlFixture fixture;
    private readonly TurretService turretService;

    public TurretServiceIntegrationTests(PostgreSqlFixture fixture)
    {
        this.fixture =fixture;
        var factory = new TestDbConnectionFactory(this.fixture.Connection.ConnectionString);
        turretService = new TurretService(factory);
    }

    public async Task InitializeAsync()
    {
        await fixture.Connection.ExecuteAsync("TRUNCATE turrets, planets, users RESTART IDENTITY CASCADE;");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateTurret_ShouldInsertAndReturnTurret()
    {
        var userId = Guid.NewGuid();
        await fixture.Connection.ExecuteAsync(@"
            INSERT INTO users (id, google_id, email) 
            VALUES (@Id, @GoogleId, @Email);", new
        {
            Id = userId,
            GoogleId = $"test-{userId}",
            Email = $"user-{userId}@example.com"
        });

        var planetId = await fixture.Connection.ExecuteScalarAsync<int>(@"
            INSERT INTO planets (user_id, x, y, resource_reserve) 
            VALUES (@UserId, 5, 5, 100) 
            RETURNING id;", new { UserId = userId });

        var turret = await turretService.CreateTurret(planetId);

        var inserted = await fixture.Connection.QuerySingleAsync<Turret>(
            "SELECT * FROM turrets WHERE id = @Id",
            new { turret.Id });

        Assert.NotNull(inserted);
        Assert.Equal(1, inserted.Level);
    }

    [Fact]
    public async Task UpgradeTurret_ShouldIncrementLevel()
    {
        var userId = Guid.NewGuid();
        await fixture.Connection.ExecuteAsync(@"
            INSERT INTO users (id, google_id, email) 
            VALUES (@Id, @GoogleId, @Email);", new
        {
            Id = userId,
            GoogleId = $"test-{userId}",
            Email = $"user-{userId}@example.com"
        });

        var planetId = await fixture.Connection.ExecuteScalarAsync<int>(@"
            INSERT INTO planets (user_id, x, y, resource_reserve) 
            VALUES (@UserId, 3, 4, 100) 
            RETURNING id;", new { UserId = userId });

        var turretId = await fixture.Connection.ExecuteScalarAsync<int>(@"
            INSERT INTO turrets (planet_id, level) 
            VALUES (@PlanetId, 1) 
            RETURNING id;", new { PlanetId = planetId });

        var upgraded = await turretService.UpgradeTurret(turretId, planetId, cost: 25);

        var planet = await fixture.Connection.QuerySingleAsync<Planet>(
            "SELECT * FROM planets WHERE id = @Id", new { Id = planetId });

        var turret = await fixture.Connection.QuerySingleAsync<Turret>(
            "SELECT * FROM turrets WHERE id = @Id", new { Id = turretId });

        Assert.Equal(2, turret.Level);
    }
}
