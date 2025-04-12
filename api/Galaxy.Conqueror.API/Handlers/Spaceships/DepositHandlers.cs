using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers.Spaceships;

public class DepositHandlers {
    
    public static async Task<IResult> DepositResourcesHandler(
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

            var planetWithDeposit = await spaceshipService.Deposit(spaceship.Id, planet.Id, spaceship.ResourceReserve);

            return Results.Ok(planetWithDeposit);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refueling spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
