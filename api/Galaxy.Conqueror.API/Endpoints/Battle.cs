using Microsoft.AspNetCore.Mvc;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;


namespace Galaxy.Conqueror.API.Endpoints;

//Attack Planet
//Pull necessary data and initiate battle

public static class AttackPlanet
{
    public static IEndpointRouteBuilder Battle(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/battle/{planetId:int}", BattleHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        endpoint.MapPost("api/battle/{planetId:int}/log", BattleLogHandler)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);

        return endpoint;

    }

    public static async Task<IResult> BattleHandler(
        [FromRoute] int planetId,
        [FromServices] UserService userService,
        [FromServices] SpaceshipService spaceshipService,
        [FromServices] PlanetService planetService,
        HttpContext context,
        CancellationToken ct
        )
    {
        try
        {
            //var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            //if (string.IsNullOrEmpty(email))
            //    return Results.BadRequest("Email claim not found in token.");
            // testing '
            var email = "user1@example.com";

            var user = await userService.GetUserByEmail(email);
            if (user == null)
                return Results.NotFound("User not found.");
            var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);
            if (spaceship == null)
                return Results.NotFound("Spaceship not found.");
            var planet = await planetService.GetPlanetById(planetId);
            if (planet == null)
                return Results.NotFound("Planet not found.");
            var turret = await planetService.GetTurretByPlanetId(planetId);
            if (turret == null)
                return Results.NotFound("Planet has no defenses");

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["planetResourceReserve"] = planet.ResourceReserve.ToString(),
                ["turretHealth"] = Calculations.getTurretHealth(turret.Level).ToString(),
                ["turretDamage"] = Calculations.getTurretDamage(turret.Level).ToString(),
                ["spaceshipMaxResources"] = Calculations.getSpaceshipMaxResources(spaceship.Level).ToString(),
                ["spaceshipResourceReserve"] = (spaceship.ResourceReserve).ToString(),
                ["spaceshipHealth"] = (spaceship.CurrentHealth).ToString(),
                ["spaceshipDamage"] = Calculations.getSpaceshipDamage(spaceship.Level).ToString(),
                ["spaceshipDesign"] = (spaceship.Design)
            };
            return Results.Ok(dict);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving battle details: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

        public static async Task<IResult> BattleLogHandler(
        [FromRoute] int planetId
        )
    {
        Console.WriteLine(planetId);
        await Task.Delay(1000);
        return Results.BadRequest("Not Implemented");
    }
}
