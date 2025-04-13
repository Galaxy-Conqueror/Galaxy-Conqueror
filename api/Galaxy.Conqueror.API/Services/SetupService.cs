using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;
using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Services;

public class SetupService(
    IDbConnectionFactory connectionFactory,
    PlanetService planetService,
    ResourceExtractorService resourceExtractorService,
    TurretService turretService,
    SpaceshipService spaceshipService,
    ILogger<SetupService> logger)
{
    public async Task<User> SetupPlayerDefaults(string email, string googleId, string username)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {

            const string insertSql = @"
                INSERT INTO users (email, google_id, username)
                VALUES (@Email, @GoogleId, @Username)
                RETURNING *";

            var newUser = new
            {
                Email = email,
                GoogleId = googleId,
                Username = username
            };

            var user = await connection.QuerySingleOrDefaultAsync<User>(insertSql, newUser, transaction);
            
            // TODO generate random position for planet
            var planet = await planetService.CreatePlanet(user.Id, transaction);
            var resourceExtractor = await resourceExtractorService.CreateResourceExtractor(planet.Id, transaction);
            var turret = turretService.CreateTurret(planet.Id, transaction);
            var spaceship = await spaceshipService.CreateSpaceship(user.Id, planet, transaction);

            await transaction.CommitAsync();
            logger.LogInformation("Successfully setup new user");
            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting up new user.");
            await transaction.RollbackAsync();
            throw;
        }
    }
}
