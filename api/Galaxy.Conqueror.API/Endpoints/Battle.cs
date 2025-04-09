using Microsoft.AspNetCore.Mvc;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
using Galaxy.Conqueror.API.Models.Requests;
using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Endpoints;

public static class Battles
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
                ["turretHealth"] = Calculations.GetTurretHealth(turret.Level).ToString(),
                ["turretDamage"] = Calculations.GetTurretDamage(turret.Level).ToString(),
                ["spaceshipMaxResources"] = Calculations.GetSpaceshipMaxResources(spaceship.Level).ToString(),
                ["spaceshipResourceReserve"] = (spaceship.ResourceReserve).ToString(),
                ["spaceshipHealth"] = (spaceship.CurrentHealth).ToString(),
                ["spaceshipDamage"] = Calculations.GetSpaceshipDamage(spaceship.Level).ToString(),
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
    [FromRoute] int planetId,
    [FromBody] BattleLogRequest battleLog,
    [FromServices] UserService userService,
    [FromServices] SpaceshipService spaceshipService,
    [FromServices] PlanetService planetService,
    [FromServices] BattleService battleService,
    HttpContext context,
    CancellationToken ct
    )
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
        bool attackerWon = (Calculations.GetTurretHealth(turret.Level) - battleLog.DamageToTurret) == 0;

        Console.WriteLine($"Planet ID: {planetId}");
        Console.WriteLine($"Started At: {battleLog.StartedAt}");
        Console.WriteLine($"Ended At: {battleLog.EndedAt}");
        Console.WriteLine($"Damage to Spaceship: {battleLog.DamageToSpaceship}");
        Console.WriteLine($"Spaceship health: {spaceship.CurrentHealth}");
        Console.WriteLine($"Damage to Turret: {battleLog.DamageToTurret}");
        Console.WriteLine($"Turret health: {Calculations.GetTurretHealth(turret.Level)}");
        Console.WriteLine($"Resources Looted: {battleLog.ResourcesLooted}");

        Console.WriteLine($"Attacker Won: {attackerWon}");

        if (attackerWon) {
            await spaceshipService.LootResources(spaceship.Id, planetId, battleLog.ResourcesLooted);
            await spaceshipService.UpdateSpaceshipHealth(spaceship.Id, battleLog.DamageToSpaceship);
        } else {
            var attackerPlanet = await planetService.GetPlanetBySpaceshipId(spaceship.Id);
            if (attackerPlanet == null) {
                throw new Exception("No planet found for spaceship");
            }
            await spaceshipService.ResetSpaceship(spaceship.Id, attackerPlanet );
        }
        var battle = await battleService.CreateBattle(user.Id, spaceship.Id, planetId, battleLog, attackerWon);
        return Results.Ok(battle);

    }
}
