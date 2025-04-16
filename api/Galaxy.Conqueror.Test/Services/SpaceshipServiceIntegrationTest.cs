using System;
using Dapper;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Services;

namespace Galaxy.Conqueror.Tests.Integration.Services
{
    public class SpaceshipServiceIntegrationTests :IClassFixture<PostgreSqlFixture>, IAsyncLifetime
    {
        private readonly PostgreSqlFixture fixture;
        private readonly SpaceshipService spaceshipService;

        public SpaceshipServiceIntegrationTests(PostgreSqlFixture fixture)
        {
            this.fixture = fixture;
            var factory = new TestDbConnectionFactory(this.fixture.Connection.ConnectionString);
            spaceshipService = new SpaceshipService(factory);
        }

        public async Task InitializeAsync()
        {
            await fixture.Connection.ExecuteAsync("TRUNCATE spaceships, planets, users RESTART IDENTITY CASCADE;");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task CreateSpaceship_ShouldInsertAndReturnSpaceship()
        {
            // Arrange
            var userId = Guid.NewGuid();
            await fixture.Connection.ExecuteAsync(
                "INSERT INTO users (id, google_id, email) VALUES (@Id, @GoogleId, @Email);",
                new
                {
                    Id = userId,
                    GoogleId = $"test-{userId}",
                    Email = $"user-{userId}@example.com"
                });

            var planet = new Planet
            {
                X = 5,
                Y = 10
            };

            await fixture.Connection.ExecuteAsync(
                "INSERT INTO planets (user_id, x, y, resource_reserve) VALUES (@UserId, @X, @Y, 100);",
                new { UserId = userId, X = planet.X, Y = planet.Y });

            // Act
            var created = await spaceshipService.CreateSpaceship(userId, planet);

            // Assert: fetch from DB
            var dbSpaceship = await fixture.Connection.QuerySingleAsync<Spaceship>(
                "SELECT * FROM spaceships WHERE id = @Id", new { created.Id });

            Assert.NotNull(dbSpaceship);
            Assert.Equal(planet.X, dbSpaceship.X);
            Assert.Equal(planet.Y + 1, dbSpaceship.Y);
            Assert.Equal(1, dbSpaceship.Level);
        }


        [Fact]
        public async Task MoveSpaceship_ShouldUpdateCoordinates()
        {
            // Arrange
            var userId = Guid.NewGuid();
            await fixture.Connection.ExecuteAsync(
                "INSERT INTO users (id, google_id, email) VALUES (@Id, @GoogleId, @Email);",
                new
                {
                    Id = userId,
                    GoogleId = $"test-{userId}",
                    Email = $"user-{userId}@example.com"
                });

            var planet = new Planet { X = 3, Y = 4 };
            await fixture.Connection.ExecuteAsync(
                "INSERT INTO planets (user_id, x, y, resource_reserve) VALUES (@UserId, @X, @Y, 100);",
                new { UserId = userId, X = planet.X, Y = planet.Y });

            var spaceship = await spaceshipService.CreateSpaceship(userId, planet);

            // Act
            await spaceshipService.MoveSpaceship(spaceship.Id, 5, 10, 20);

            // Assert: fetch fresh from DB to verify the update
            var dbResult = await fixture.Connection.QuerySingleAsync<Spaceship>(
                "SELECT * FROM spaceships WHERE id = @Id",
                new { Id = spaceship.Id });

            Assert.NotNull(dbResult);
            Assert.Equal(10, dbResult.X);
            Assert.Equal(20, dbResult.Y);
        }

    }
}
