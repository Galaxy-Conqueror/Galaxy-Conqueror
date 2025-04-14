using Galaxy.Conqueror.API.Models.Responses;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers.Spaceships;

public class UpgradeHandlers {

    public static async Task<IResult> GetSpaceshipUpgradeDetailsHandler(
        [FromServices] UserService userService,
        [FromServices] SpaceshipService spaceshipService,
        HttpContext context,
        CancellationToken ct
    )
    {
        try
        {
            var user = await userService.GetUserByContext(context);
            if (user == null)
                return Results.NotFound("User not found.");

            var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);
            if (spaceship == null)
                return Results.NotFound("Spaceship not found.");

            SpaceshipUpgradeResponse upgradeInfo = new()
            {
                Level = spaceship.Level,
                UpgradeCost = Calculations.GetSpaceshipUpgradeCost(spaceship.Level),
                Damage = Calculations.GetSpaceshipDamage(spaceship.Level),
                MaxHealth = Calculations.GetSpaceshipMaxHealth(spaceship.Level),
                MaxFuel = Calculations.GetSpaceshipMaxFuel(spaceship.Level),
                MaxResources = Calculations.GetSpaceshipMaxResources(spaceship.Level),
                UpgradedDamage = Calculations.GetSpaceshipDamage(spaceship.Level + 1),
                UpgradedMaxHealth = Calculations.GetSpaceshipMaxHealth(spaceship.Level + 1),
                UpgradedMaxFuel = Calculations.GetSpaceshipMaxFuel(spaceship.Level + 1),
                UpgradedMaxResources = Calculations.GetSpaceshipMaxResources(spaceship.Level + 1),
                UpgradedLevel = spaceship.Level + 1,
            };

            return Results.Ok(upgradeInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving upgrade details: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> UpgradeSpaceshipHandler(
        [FromServices] UserService userService,
        [FromServices] SpaceshipService spaceshipService,
        [FromServices] PlanetService planetService,
        HttpContext context,
        CancellationToken ct
    )
    {
        try
        {
            var user = await userService.GetUserByContext(context);
            if (user == null)
                return Results.NotFound("User not found.");

            var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);
            if (spaceship == null)
                return Results.NotFound("Spaceship not found.");
            
            var planet = await planetService.GetPlanetByUserID(user.Id);
            if (planet == null)
                return Results.NotFound("Planet not found.");

            if (!Calculations.IsInRange(spaceship.X, spaceship.Y, planet.X, planet.Y, 1))
                return Results.BadRequest("Not close enough to home planet");

            var upgradeCost = Calculations.GetSpaceshipUpgradeCost(spaceship.Level);
            if (planet.ResourceReserve < upgradeCost)
                return Results.BadRequest("Error upgrading spaceship: Insufficient resources");

            var upgradedSpaceship = await spaceshipService.UpdateSpaceship(spaceship.Id, planet.Id, upgradeCost);

            SpaceshipUpgradedResponse upgradedSpaceshipResponse = new()
            {
                Level = upgradedSpaceship.Level,
                Damage = Calculations.GetSpaceshipDamage(upgradedSpaceship.Level),
                MaxHealth = Calculations.GetSpaceshipMaxHealth(upgradedSpaceship.Level),
                MaxResources = Calculations.GetSpaceshipMaxResources(upgradedSpaceship.Level),
                MaxFuel = Calculations.GetSpaceshipMaxFuel(upgradedSpaceship.Level),
                PlanetResourceReserve = planet.ResourceReserve - upgradeCost
            };

            return Results.Ok(upgradedSpaceshipResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error upgrading spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
}
