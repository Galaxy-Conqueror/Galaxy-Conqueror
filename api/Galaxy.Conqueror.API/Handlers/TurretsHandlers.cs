using Galaxy.Conqueror.API.Models.Responses;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers;

public class TurretsHandlers {
    
    public static async Task<IResult> GetTurretDetailsHandler(
        [FromServices] IUserService userService,
        [FromServices] TurretService turretService,
        HttpContext context
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

            TurretDetailResponse response = new()
            {
                Id = turret.Id,
                PlanetId = turret.PlanetId,
                Level = turret.Level,
                Damage = Calculations.GetTurretDamage(turret.Level),
                Health = Calculations.GetTurretHealth(turret.Level),
                UpgradeCost = Calculations.GetTurretUpgradeCost(turret.Level),
                UpgradedDamage = Calculations.GetTurretDamage(turret.Level + 1),
                UpgradedHealth = Calculations.GetTurretHealth(turret.Level + 1)
            };

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting turret upgrade details: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> UpgradeTurretHandler(
        [FromServices] IUserService userService,
        [FromServices] SpaceshipService spaceshipService,
        [FromServices] PlanetService planetService,
        [FromServices] TurretService turretService,
        HttpContext context
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

            var upgradeCost = Calculations.GetTurretUpgradeCost(turret.Level);
            if (planet.ResourceReserve < upgradeCost)
                return Results.BadRequest("Error upgrading turret: Insufficient resources");

            var upgradedTurret = await turretService.UpgradeTurret(turret.Id, planet.Id, upgradeCost);

            TurretUpgradedResponse upgradedTurretResponse = new()
            {
                Level = upgradedTurret.Level,
                Damage = Calculations.GetTurretDamage(upgradedTurret.Level),
                Health = Calculations.GetTurretHealth(upgradedTurret.Level),
                PlanetResourceReserve = planet.ResourceReserve - upgradeCost
            };

            return Results.Ok(upgradedTurretResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error upgrading turret: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
