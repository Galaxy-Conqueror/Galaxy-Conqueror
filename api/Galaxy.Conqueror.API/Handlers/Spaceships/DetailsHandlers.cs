using Galaxy.Conqueror.API.Models.Responses;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
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
            if (spaceship == null)
                return Results.NotFound("Spaceship not found.");

            SpaceshipResponse spaceshipResponse = new()
            {
                Id = spaceship.Id,
                UserId = spaceship.UserId,
                Design = spaceship.Design,
                Description = spaceship.Description,
                Level = spaceship.Level,
                CurrentFuel = spaceship.CurrentFuel,
                CurrentHealth = spaceship.CurrentHealth,
                ResourceReserve = spaceship.ResourceReserve,
                X = spaceship.X,
                Y = spaceship.Y,
                Damage = Calculations.GetSpaceshipDamage(spaceship.Level),
                MaxHealth = Calculations.GetSpaceshipMaxHealth(spaceship.Level),
                MaxResources = Calculations.GetSpaceshipMaxResources(spaceship.Level),
                MaxFuel = Calculations.GetSpaceshipMaxFuel(spaceship.Level)
            };

            return Results.Ok(spaceshipResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
