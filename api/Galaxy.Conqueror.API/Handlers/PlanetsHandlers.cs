using Galaxy.Conqueror.API.Models.Requests;
using Galaxy.Conqueror.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers;

public class PlanetsHandlers {
    
   public static async Task<IResult> GetAllPlanetDetailsHandler(
        [FromServices] PlanetService planetService
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
        [FromServices] IUserService userService,
        [FromServices] PlanetService planetService,
        HttpContext context
    )
    {
        try
        {
            var user = await userService.GetUserByContext(context);
            if (user == null)
                return Results.NotFound("User not found.");

            var planet = await planetService.GetPlanetByUserID(user.Id);
            if (planet == null)
                return Results.NotFound("Planet not found.");

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
        [FromServices] PlanetService planetService
    )
    {
        try
        {
            var planet = await planetService.GetPlanetById(planetId);
            if (planet == null) 
                return Results.NotFound("Planet not found.");

            return Results.Ok(planet);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving planet: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> UpdatePlanetNameHandler(
        [FromServices] IUserService userService,
        [FromServices] PlanetService planetService,
        [FromServices] AiService aiService,
        [FromBody] PlanetNameRequest request,
        HttpContext context
    )
    {
        try
        {
            var user = await userService.GetUserByContext(context);
            if (user == null)
                return Results.NotFound("User not found.");

            var planet = await planetService.UpdatePlanetName(user.Id, request.PlanetName, aiService);

            return Results.Ok(planet);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving planet: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
