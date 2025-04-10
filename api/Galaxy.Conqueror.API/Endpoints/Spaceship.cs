using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Galaxy.Conqueror.API.Endpoints;

public static class Spaceship
{
    public static IEndpointRouteBuilder ViewSpaceshipDetails(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship", ViewSpaceshipDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();
        return endpoint;
    }

    public static async Task<IResult> ViewSpaceshipDetailsHandler(
        [FromServices] UserService userService,
        [FromServices] SpaceshipService spaceshipService,
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

            return Results.Ok(spaceship);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static IEndpointRouteBuilder ViewSpaceshipUpgradeDetails(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship/upgrade", ViewSpaceshipUpgradeDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();
        return endpoint;
    }

    public static async Task<IResult> ViewSpaceshipUpgradeDetailsHandler(
    [FromServices] UserService userService,
    [FromServices] SpaceshipService spaceshipService,
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

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["upgradeCost"] = Calculations.GetSpaceshipUpgradeCost(spaceship.Level).ToString(),
                ["upgradeEffect"] = Calculations.GetSpaceshipUpgradeEffect(spaceship.Level).ToString()
            };

            return Results.Ok(dict);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static IEndpointRouteBuilder UpgradeSpaceship(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("api/spaceship/upgrade", UpgradeSpaceshipHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();
        return endpoint;
    }

    public static async Task<IResult> UpgradeSpaceshipHandler(
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

            var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);

            if (spaceship == null)
                return Results.BadRequest("Spaceship not found.");
            
            var planet = await planetService.GetPlanetByUserID(user.Id);

            if (planet == null)
                return Results.BadRequest("Planet not found.");

            // Validation to check whether the spaceship is in a 1 block distance from their planet
            int distance = Math.Abs(spaceship.X - planet.X) + Math.Abs(spaceship.Y - planet.Y);

            if (distance < 1)
            {
                return Results.BadRequest("Error upgrading spaceship: Planet out of range");
            }

            var upgradeCost = Calculations.GetSpaceshipUpgradeCost(spaceship.Level);

            // Validation to check if planet has enough resources for upgrade
            if (planet.ResourceReserve < upgradeCost)
            {
                return Results.BadRequest("Error upgrading spaceship: Insufficient resources");
            }

            var upgradedSpaceship = await spaceshipService.UpdateSpaceship(spaceship.Id, planet.Id, upgradeCost);

            return Results.Ok(upgradedSpaceship);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error upgrading spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static IEndpointRouteBuilder Refuel(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship/refuel", RefuelPriceHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        endpoint.MapPut("api/spaceship/refuel", RefuelSpaceshipHandler)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);

        return endpoint;

    }

    public static async Task<IResult> RefuelPriceHandler(
        [FromServices] UserService userService,
        [FromServices] SpaceshipService spaceshipService,
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

            var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);

            if (spaceship == null)
                return Results.BadRequest("Spaceship not found.");

            return Results.Ok(Calculations.GetSpaceshipRefuelCost(spaceship.Level, spaceship.CurrentFuel));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refueling spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> RefuelSpaceshipHandler(
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

            var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);

            if (spaceship == null)
                return Results.BadRequest("Spaceship not found.");

            var planet = await planetService.GetPlanetByUserID(user.Id);

            if (planet == null)
                return Results.BadRequest("Planet not found.");

            if (!Calculations.IsInRange(spaceship.X, spaceship.Y, planet.X, planet.Y, 1)) {
                return Results.BadRequest("Not close enough to home planet");
            }

            int planetResources = planet.ResourceReserve;
            int refuelCost = Calculations.GetSpaceshipRefuelCost(spaceship.Level, spaceship.CurrentFuel);

            if (refuelCost > planetResources) {
                return Results.BadRequest("Not enough resources to refuel");
            }

            var refueledSpaceship = await spaceshipService.Refuel(spaceship.Id, planet.Id, refuelCost, Calculations.GetSpaceshipMaxFuel(spaceship.Level) - spaceship.CurrentFuel);

            return Results.Ok(refueledSpaceship);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refueling spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static IEndpointRouteBuilder Repair(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship/repair", RepairPriceHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        endpoint.MapPut("api/spaceship/repair", RepairSpaceshipHandler)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        return endpoint;

    }

    public static async Task<IResult> RepairPriceHandler(
        [FromServices] UserService userService,
        [FromServices] SpaceshipService spaceshipService,
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

            var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);

            if (spaceship == null)
                return Results.BadRequest("Spaceship not found.");

            return Results.Ok(Calculations.GetSpaceshipRepairCost(spaceship.Level, spaceship.CurrentHealth));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error repairing spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> RepairSpaceshipHandler(
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

            var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);

            if (spaceship == null)
                return Results.BadRequest("Spaceship not found.");

            var planet = await planetService.GetPlanetByUserID(user.Id);

            if (planet == null)
                return Results.BadRequest("Planet not found.");

            if (!Calculations.IsInRange(spaceship.X, spaceship.Y, planet.X, planet.Y, 1)) {
                return Results.BadRequest("Not close enough to home planet");
            }

            int planetResources = planet.ResourceReserve;
            int repairCost = Calculations.GetSpaceshipRepairCost(spaceship.Level, spaceship.CurrentHealth);

            if (repairCost > planetResources) {
                return Results.BadRequest("Not enough resources to refuel");
            }

            var repairedSpaceship = await spaceshipService.Repair(spaceship.Id, planet.Id, repairCost, Calculations.GetSpaceshipMaxHealth(spaceship.Level) - spaceship.CurrentHealth);

            return Results.Ok(repairedSpaceship);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refueling spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static IEndpointRouteBuilder Move(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPut("api/spaceship/move", MoveSpaceshipHandler)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        return endpoint;
    }

    public static async Task<IResult> MoveSpaceshipHandler(
        [FromServices] UserService userService,
        [FromServices] SpaceshipService spaceshipService,
        [FromServices] PlanetService planetService,
        [FromQuery] int x,
        [FromQuery] int y,
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

            var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);

            if (spaceship == null)
                return Results.BadRequest("Spaceship not found.");

            int fuelUsed = Calculations.GetFuelUsed(spaceship.X, spaceship.Y, x, y);
            if (fuelUsed > spaceship.CurrentFuel)
            {
                return Results.BadRequest("Not enough fuel to reach destination");
            }

            var movedSpaceship = await spaceshipService.MoveSpaceship(spaceship.Id, fuelUsed, x, y);

            return Results.Ok(movedSpaceship);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error moving spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }



    public static IEndpointRouteBuilder Deposit(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPut("api/spaceship/deposit", DepositResourcesHandler)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        return endpoint;

    }
    public static async Task<IResult> DepositResourcesHandler(
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

            var spaceship = await spaceshipService.GetSpaceshipByUserId(user.Id);

            if (spaceship == null)
                return Results.BadRequest("Spaceship not found.");

            var planet = await planetService.GetPlanetByUserID(user.Id);

            if (planet == null)
                return Results.BadRequest("Planet not found.");

            if (!Calculations.IsInRange(spaceship.X, spaceship.Y, planet.X, planet.Y, 1)) {
                return Results.BadRequest("Not close enough to home planet");
            }

            var planetWithDeposit = await spaceshipService.Deposit(spaceship.Id, planet.Id, spaceship.ResourceReserve);

            return Results.Ok(planetWithDeposit);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refueling spaceship: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
