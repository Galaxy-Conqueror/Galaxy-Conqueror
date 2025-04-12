using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers;

public class TurretsHandlers {
    
    public static async Task<IResult> GetTurretDetailsHandler(
        [FromServices] UserService userService,
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

            var turret = await turretService.GetTurretByUserID(user.Id);
            if (turret == null)
                return Results.NotFound("Turret not found.");

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["level"] = turret.Level.ToString(),
                ["damage"] = Calculations.GetTurretDamage(turret.Level).ToString(),
                ["health"] = Calculations.GetTurretHealth(turret.Level).ToString(),
                ["upgradeCost"] = Calculations.GetTurretUpgradeCost(turret.Level).ToString(),
                ["upgradedDamage"] = Calculations.GetTurretDamage(turret.Level + 1).ToString(),
                ["upgradedHealth"] = Calculations.GetTurretHealth(turret.Level + 1).ToString()
            };

            return Results.Ok(dict);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting turret upgrade details: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> UpgradeTurretHandler(
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
            
            var planet = await planetService.GetPlanetByUserID(user.Id);

            if (planet == null)
                return Results.NotFound("Planet not found.");

            var turret = await turretService.GetTurretByUserID(user.Id);
            if (turret == null)
                return Results.NotFound("Turret not found.");

            if (!Calculations.IsInRange(spaceship.X, spaceship.Y, planet.X, planet.Y, 1))
                return Results.BadRequest("Error upgrading turret: Planet out of range");

            var upgradeCost = Calculations.GetExtractorUpgradeCost(turret.Level);

            // Validation to check if planet has enough resources for upgrade
            if (planet.ResourceReserve < upgradeCost)
                return Results.BadRequest("Error upgrading turret: Insufficient resources");

            var upgradedTurret = await turretService.UpgradeTurret(turret.Id, planet.Id, upgradeCost);

            return Results.Ok(upgradedTurret);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error upgrading turret: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
