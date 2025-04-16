using System;
using System.Threading.Tasks;
using Dapper;
using Moq;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Services;
using Microsoft.Extensions.Logging;

namespace Galaxy.Conqueror.Tests.Integration.Services;

public class SetupServiceIntegrationTests : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    private readonly PostgreSqlFixture fixture;
    private readonly SetupService setupService;

    public SetupServiceIntegrationTests(PostgreSqlFixture fixture)
    {
        this.fixture = fixture;
        var factory = new TestDbConnectionFactory(this.fixture.Connection.ConnectionString);

        var planetService = new PlanetService(factory);
        var spaceshipService = new SpaceshipService(factory);
        var resourceExtractorService = new ResourceExtractorService(factory);
        var turretService = new TurretService(factory);

        var aiServiceMock = new Mock<IAiService>();
        aiServiceMock
            .Setup(a => a.AiGeneratorAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync("Mocked AI response");

        var logger = new LoggerFactory().CreateLogger<SetupService>();

        setupService = new SetupService(
            factory,
            planetService,
            spaceshipService,
            resourceExtractorService,
            turretService,
            aiServiceMock.Object,
            logger);
    }

    public async Task InitializeAsync()
    {
        await fixture.Connection.ExecuteAsync("TRUNCATE battles, spaceships, turrets, resource_extractors, planets, users RESTART IDENTITY CASCADE;");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task SetupPlayerDefaults_ShouldInsertUserAndCreateDependencies()
    {
        string email = "player1@example.com";
        string googleId = Guid.NewGuid().ToString();
        string username = "SpaceRanger";

        var user = await setupService.SetupPlayerDefaults(email, googleId, username);

        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(username, user.Username);

        var planet = await fixture.Connection.QuerySingleOrDefaultAsync<Planet>(
            "SELECT * FROM planets WHERE user_id = @UserId", new { UserId = user.Id });
        Assert.NotNull(planet);

        var extractor = await fixture.Connection.QuerySingleOrDefaultAsync<ResourceExtractor>(
            "SELECT * FROM resource_extractors WHERE planet_id = @PlanetId", new { PlanetId = planet.Id });
        Assert.NotNull(extractor);

        var turret = await fixture.Connection.QuerySingleOrDefaultAsync<Turret>(
            "SELECT * FROM turrets WHERE planet_id = @PlanetId", new { PlanetId = planet.Id });
        Assert.NotNull(turret);

        var spaceship = await fixture.Connection.QuerySingleOrDefaultAsync<Spaceship>(
            "SELECT * FROM spaceships WHERE user_id = @UserId", new { UserId = user.Id });
        Assert.NotNull(spaceship);
    }
}
