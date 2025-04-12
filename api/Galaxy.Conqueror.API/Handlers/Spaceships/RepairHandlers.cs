using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers.Spaceships;

public class RepairHandlers {

    public static async Task<IResult> RepairPriceHandler(
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
                return Results.BadRequest("Spaceship not found.");

            return Results.Ok(Calculations.GetSpaceshipRepairCost(spaceship.Level, spaceship.CurrentHealth));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error repairing spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> RepairSpaceshipHandler(
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

            if (!Calculations.IsInRange(spaceship.X, spaceship.Y, planet.X, planet.Y, 1)) {
                return Results.BadRequest("Not close enough to home planet");
            }

            int planetResources = planet.ResourceReserve;
            int repairCost = Calculations.GetSpaceshipRepairCost(spaceship.Level, spaceship.CurrentHealth);

            if (repairCost > planetResources) {
                return Results.BadRequest("Not enough resources to refuel");
            }

            var repairedSpaceship = await spaceshipService.Repair(spaceship.Id, planet.Id, repairCost, Calculations.GetSpaceshipMaxHealth(spaceship.Level) - spaceship.CurrentHealth);

            return Results.Ok(repairedSpaceship);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refueling spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
}
