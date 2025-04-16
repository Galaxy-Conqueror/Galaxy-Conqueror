using System;
using Dapper;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Services;

namespace Galaxy.Conqueror.Tests.Integration.Services;

public class ResourceExtractorServiceIntegrationTests : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    private readonly PostgreSqlFixture fixture;
    private readonly ResourceExtractorService resourceExtractorService;

    public ResourceExtractorServiceIntegrationTests(PostgreSqlFixture fixture)
    {
        this.fixture = fixture;
        var factory = new TestDbConnectionFactory(this.fixture.Connection.ConnectionString);
        resourceExtractorService = new ResourceExtractorService(factory);
    }

    public async Task InitializeAsync()
    {
        await fixture.Connection.ExecuteAsync("TRUNCATE resource_extractors, planets, users RESTART IDENTITY CASCADE;");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateResourceExtractor_ShouldInsertAndReturnExtractor()
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
            VALUES (@UserId, 1, 2, 100) 
            RETURNING id;", new { UserId = userId });

        var extractor = await resourceExtractorService.CreateResourceExtractor(planetId);

        var inserted = await fixture.Connection.QuerySingleAsync<ResourceExtractor>(
            "SELECT * FROM resource_extractors WHERE id = @Id",
            new { extractor.Id });

        Assert.NotNull(inserted);
        Assert.Equal(1, inserted.Level);
    }

    [Fact]
    public async Task UpgradeResourceExtractor()
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
            VALUES (@UserId, 1, 2, 100) 
            RETURNING id;", new { UserId = userId });

        var extractorId = await fixture.Connection.ExecuteScalarAsync<int>(@"
            INSERT INTO resource_extractors (planet_id, level) 
            VALUES (@PlanetId, 1) 
            RETURNING id;", new { PlanetId = planetId });

        var planet = await fixture.Connection.QuerySingleAsync<Planet>(
            "SELECT * FROM planets WHERE id = @Id", new { Id = planetId });

        var upgraded = await resourceExtractorService.UpgradeResourceExtractor(extractorId, planetId, 10);


        var extractor = await fixture.Connection.QuerySingleAsync<ResourceExtractor>(
            "SELECT * FROM resource_extractors WHERE id = @Id", new { Id = extractorId });

        Assert.Equal(2, extractor.Level);
    }
}
