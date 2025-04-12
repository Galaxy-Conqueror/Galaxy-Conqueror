using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers.Spaceships;

public class MoveHandlers {

    public static async Task<IResult> MoveSpaceshipHandler(
        [FromServices] UserService userService,
        [FromServices] SpaceshipService spaceshipService,
        [FromServices] PlanetService planetService,
        [FromQuery] int x,
        [FromQuery] int y,
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

            int fuelUsed = Calculations.GetFuelUsed(spaceship.X, spaceship.Y, x, y);
            if (fuelUsed > spaceship.CurrentFuel)
            {
                return Results.BadRequest("Not enough fuel to reach destination");
            }

            var movedSpaceship = await spaceshipService.MoveSpaceship(spaceship.Id, fuelUsed, x, y);

            return Results.Ok(movedSpaceship);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error moving spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
}
