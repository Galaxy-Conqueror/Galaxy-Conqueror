using Galaxy.Conqueror.API.Configuration.Database;

namespace Galaxy.Conqueror.API.Services;

public class SetupService(
    IDbConnectionFactory connectionFactory,
    PlanetService planetService,
    ResourceExtractorService resourceExtractorService,
    TurretService turretService,
    SpaceshipService spaceshipService,
    ILogger<SetupService> logger)
{
    public async Task SetupPlayerDefaults(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // TODO generate random position for planet
            var planet = await planetService.CreatePlanet(userId, transaction);
            var resourceExtractor = await resourceExtractorService.CreateResourceExtractor(planet.Id, transaction);
            var turret = turretService.CreateTurret(planet.Id, transaction);
            var spaceship = await spaceshipService.CreateSpaceship(userId, planet, transaction);

            await transaction.CommitAsync();
            logger.LogInformation("Successfully setup new user");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting up new user.");
            await transaction.RollbackAsync();
            throw;
        }
    }
}
