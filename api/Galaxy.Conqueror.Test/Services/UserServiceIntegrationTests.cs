using System;
using Dapper;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Models.Responses;
using Galaxy.Conqueror.API.Services;
using Microsoft.Extensions.Logging;

namespace Galaxy.Conqueror.Tests.Integration.Services;

public class UserServiceIntegrationTests : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    private readonly PostgreSqlFixture fixture;
    private readonly UserService userService;

    public UserServiceIntegrationTests(PostgreSqlFixture fixture)
    {
        this.fixture = fixture;
        var factory = new TestDbConnectionFactory(this.fixture.Connection.ConnectionString);

        // Dependencies
        var planetService = new PlanetService(factory);
        var spaceshipService = new SpaceshipService(factory);
        var resourceExtractorService = new ResourceExtractorService(factory);
        var turretService = new TurretService(factory);
        var logger = new LoggerFactory().CreateLogger<SetupService>();
        var setupService = new SetupService(factory, planetService, spaceshipService, resourceExtractorService, turretService, logger);

        userService = new UserService(factory, setupService);
    }

    public async Task InitializeAsync()
    {
        await fixture.Connection.ExecuteAsync("TRUNCATE battles, spaceships, turrets, resource_extractors, planets, users RESTART IDENTITY CASCADE;");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateUser_ShouldInsertUserAndSetupDefaults()
    {
        // Arrange
        var userInfo = new UserInfoResponse
        {
            email = "newplayer@example.com",
            sub = Guid.NewGuid().ToString()
        };

        // Act
        var user = await userService.CreateUser(userInfo);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(userInfo.email, user.Email);

        // Ensure related records were created
        var planet = await fixture.Connection.QuerySingleOrDefaultAsync<Planet>("SELECT * FROM planets WHERE user_id = @UserId", new { UserId = user.Id });

        Assert.NotNull(planet);

        var extractor = await fixture.Connection.QuerySingleOrDefaultAsync<ResourceExtractor>("SELECT * FROM resource_extractors WHERE planet_id = @PlanetId", new { PlanetId = planet.Id });
        Assert.NotNull(extractor);

        var turret = await fixture.Connection.QuerySingleOrDefaultAsync<Turret>("SELECT * FROM turrets WHERE planet_id = @PlanetId", new { PlanetId = planet.Id });
        Assert.NotNull(turret);

        var spaceship = await fixture.Connection.QuerySingleOrDefaultAsync<Spaceship>("SELECT * FROM spaceships WHERE user_id = @UserId", new { UserId = user.Id });
        Assert.NotNull(spaceship);
    }

    [Fact]
    public async Task GetUserByEmail()
    {
        // Arrange
        var userInfo = new UserInfoResponse
        {
            email = "existing@example.com",
            sub = Guid.NewGuid().ToString()
        };
        var created = await userService.CreateUser(userInfo);

        // Act
        var found = await userService.GetUserByEmail(userInfo.email);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(created!.Id, found!.Id);
    }

    [Fact]
    public async Task UpdatingTheUser()
    {
        // Arrange
        var userInfo = new UserInfoResponse
        {
            email = "updatable@example.com",
            sub = Guid.NewGuid().ToString()
        };
        var user = await userService.CreateUser(userInfo);

        // Act
        var updated = await userService.UpdateUser(user!.Id, "GalaxyMaster");

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("GalaxyMaster", updated!.Username);
    }

    [Fact]
    public async Task DeletingTheUser()
    {
        // Arrange
        var userInfo = new UserInfoResponse
        {
            email = "delete@example.com",
            sub = Guid.NewGuid().ToString()
        };
        var user = await userService.CreateUser(userInfo);

        // Act
        await userService.DeleteUser(user!.Id);

        // Assert
        var result = await userService.GetUserById(user.Id);
        Assert.Null(result);
    }
}
