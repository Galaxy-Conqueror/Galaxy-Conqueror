using Galaxy.Conqueror.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers.Spaceships;

public class DetailsHandlers {
    
public static async Task<IResult> GetSpaceshipDetailsHandler(
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

            return Results.Ok(spaceship);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
