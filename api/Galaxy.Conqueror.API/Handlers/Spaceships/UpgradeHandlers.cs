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

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["upgradeCost"] = Calculations.GetSpaceshipUpgradeCost(spaceship.Level).ToString(),
                ["upgradeEffect"] = Calculations.GetSpaceshipUpgradeEffect(spaceship.Level).ToString()
            };

            return Results.Ok(dict);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user: {ex}");
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
                return Results.BadRequest("Spaceship not found.");
            
            var planet = await planetService.GetPlanetByUserID(user.Id);
            if (planet == null)
                return Results.BadRequest("Planet not found.");

            // Validation to check whether the spaceship is in a 1 block distance from their planet
            int distance = Math.Abs(spaceship.X - planet.X) + Math.Abs(spaceship.Y - planet.Y);
            if (distance < 1)
            {
                return Results.BadRequest("Error upgrading spaceship: Planet out of range");
            }

            var upgradeCost = Calculations.GetSpaceshipUpgradeCost(spaceship.Level);

            // Validation to check if planet has enough resources for upgrade
            if (planet.ResourceReserve < upgradeCost)
            {
                return Results.BadRequest("Error upgrading spaceship: Insufficient resources");
            }

            var upgradedSpaceship = await spaceshipService.UpdateSpaceship(spaceship.Id, planet.Id, upgradeCost);

            return Results.Ok(upgradedSpaceship);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error upgrading spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
}
