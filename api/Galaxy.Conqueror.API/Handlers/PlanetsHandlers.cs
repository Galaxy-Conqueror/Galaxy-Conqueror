using Galaxy.Conqueror.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers;

public class PlanetsHandlers {
    
   public static async Task<IResult> GetAllPlanetDetailsHandler(
        [FromServices] PlanetService planetService,
        HttpContext context,
        CancellationToken ct
    )
    {
        try
        {
            var planets = await planetService.GetPlanets();
            return Results.Ok(planets);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving planets: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> GetPlanetDetailsHandler(
        [FromServices] UserService userService,
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

            var planet = await planetService.GetPlanetByUserID(user.Id);

            return Results.Ok(planet);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving planet: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> GetPlanetDetailsByIdHandler(
        [FromRoute] int planetId,
        [FromServices] UserService userService,
        [FromServices] PlanetService planetService,
        HttpContext context,
        CancellationToken ct
    )
    {
        try
        {
            var planet = await planetService.GetPlanetById(planetId);
            if (planet == null) {
                return Results.NotFound("Planet not found.");
            }

            return Results.Ok(planet);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving planet: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
