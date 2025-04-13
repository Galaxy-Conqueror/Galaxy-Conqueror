using Galaxy.Conqueror.API.Models;
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

            BattleResponse battleResponse = new()
            {
                PlanetResourceReserve = planet.ResourceReserve,
                TurretHealth = Calculations.GetTurretHealth(turret.Level),
                TurretDamage = Calculations.GetTurretDamage(turret.Level),
                SpaceshipMaxResources = Calculations.GetSpaceshipMaxResources(spaceship.Level),
                SpaceshipResourceReserve = spaceship.ResourceReserve,
                SpaceshipHealth = spaceship.CurrentHealth,
                SpaceshipDamage = Calculations.GetSpaceshipDamage(spaceship.Level),
                SpaceshipDesign = spaceship.Design ?? ""
            };

            return Results.Ok(battleResponse);
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
                return Results.NotFound("Planet has no defenses");

            bool attackerWon = (Calculations.GetTurretHealth(turret.Level) - battleLog.DamageToTurret) == 0;
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error logging battle details: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
