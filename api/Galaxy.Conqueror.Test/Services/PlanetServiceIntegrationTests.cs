using System;
using Galaxy.Conqueror.API.Services;
using Npgsql;
using Dapper;

namespace MyGame.Tests.Integration
{
    public class PlanetServiceIntegrationTests : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
    {
        private readonly PlanetService planetService;
        private readonly PostgreSqlFixture fixture;


        public PlanetServiceIntegrationTests(PostgreSqlFixture fixture)
        {
            this.fixture = fixture;
            var factory = new TestDbConnectionFactory(this.fixture.Connection.ConnectionString);
            planetService = new PlanetService(factory);
        }
        public async Task InitializeAsync()
        {
            await fixture.Connection.ExecuteAsync("TRUNCATE planets, users RESTART IDENTITY CASCADE;");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetPlanets_ReturnsInsertedPlanets()
        {
            var userId = Guid.NewGuid();
            await fixture.Connection.ExecuteAsync("TRUNCATE planets, users RESTART IDENTITY CASCADE;");

            await fixture.Connection.ExecuteAsync("""
                INSERT INTO users (id, google_id, email, username)
                VALUES (@Id, 'gid123', 'test@example.com', 'TestUser');
            """, new { Id = userId });

            await fixture.Connection.ExecuteAsync("""
                INSERT INTO planets (user_id, name, design, description, resource_reserve, x, y)
                VALUES (@UserId, 'Terra Prime', '', '', 0, 1, 1),
                       (@UserId, 'Nova Terra', '', '', 0, 2, 2);
            """, new { UserId = userId });

            var planets = await planetService.GetPlanets();

            Assert.Equal(2, planets.Count());
            Assert.Contains(planets, p => p.Name == "Terra Prime");
            Assert.Contains(planets, p => p.Name == "Nova Terra");
        }

        [Fact]
        public async Task GetPlanets_ReturnsEmpty_WhenNoPlanetsExist()
        {
            var planets = await planetService.GetPlanets();
            Assert.Empty(planets);
        }

        [Fact]
        public async Task GetPlanets_OnlyReturnsPlanetsFromAllUsers()
        {
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            await fixture.Connection.ExecuteAsync("""
                INSERT INTO users (id, google_id, email, username)
                VALUES (@Id1, 'gid1', 'user1@example.com', 'User1'),
                       (@Id2, 'gid2', 'user2@example.com', 'User2');
            """, new { Id1 = user1, Id2 = user2 });

            await fixture.Connection.ExecuteAsync("""
                INSERT INTO planets (user_id, name, design, description, resource_reserve, x, y)
                VALUES (@User1, 'Planet A', '', '', 0, 1, 1),
                       (@User2, 'Planet B', '', '', 0, 2, 2);
            """, new { User1 = user1, User2 = user2 });

            var planets = await planetService.GetPlanets();

            Assert.Equal(2, planets.Count());
            Assert.Contains(planets, p => p.Name == "Planet A");
            Assert.Contains(planets, p => p.Name == "Planet B");
        }

        [Fact]
        public async Task GetPlanets_ReturnsCorrectCoordinates()
        {
            var userId = Guid.NewGuid();

            await fixture.Connection.ExecuteAsync("""
                INSERT INTO users (id, google_id, email, username)
                VALUES (@Id, 'gid1234', 'test2@example.com', 'CoordTester');
            """, new { Id = userId });

            await fixture.Connection.ExecuteAsync("""
                INSERT INTO planets (user_id, name, design, description, resource_reserve, x, y)
                VALUES (@UserId, 'Xylo', '', '', 0, 5, 7);
            """, new { UserId = userId });

            var planets = await planetService.GetPlanets();
            var xylo = planets.FirstOrDefault(p => p.Name == "Xylo");

            Assert.NotNull(xylo);
            Assert.Equal(5, xylo!.X);
            Assert.Equal(7, xylo.Y);
        }
    }
}
