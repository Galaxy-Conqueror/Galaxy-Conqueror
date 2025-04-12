using Galaxy.Conqueror.API.Models.Requests;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers;

public class BattleHandlers {
    
    public static async Task<IResult> BattleHandler(
        [FromRoute] int planetId,
        [FromServices] UserService userService,
        [FromServices] SpaceshipService spaceshipService,
        [FromServices] PlanetService planetService,
        [FromServices] TurretService turretService,
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

            var planet = await planetService.GetPlanetById(planetId);
            if (planet == null)
                return Results.NotFound("Planet not found.");

            var turret = await turretService.GetTurretByPlanetId(planetId);
            if (turret == null)
                return Results.NotFound("Planet has no turret");

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
    [FromServices] TurretService turretService,
    [FromServices] BattleService battleService,
    HttpContext context,
    CancellationToken ct
    )
    {

       var user = await userService.GetUserByContext(context);
        if (user == null)
            return Results.NotFound("User not found.");

        var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);
        if (spaceship == null)
            return Results.NotFound("Spaceship not found.");

        var planet = await planetService.GetPlanetById(planetId);
        if (planet == null)
            return Results.NotFound("Planet not found.");

        var turret = await turretService.GetTurretByPlanetId(planetId);
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
            if (attackerPlanet == null)
                throw new Exception("No planet found for spaceship");

            await spaceshipService.ResetSpaceship(spaceship.Id, attackerPlanet);
        }
        var battle = await battleService.CreateBattle(user.Id, spaceship.Id, planetId, battleLog, attackerWon);
        return Results.Ok(battle);
    }
}
