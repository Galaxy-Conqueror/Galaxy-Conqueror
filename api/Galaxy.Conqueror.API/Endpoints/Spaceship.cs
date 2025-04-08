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
                ["upgradeCost"] = Calculations.getSpaceshipUpgradeCost(spaceship.Level).ToString(),
                ["upgradeEffect"] = Calculations.getSpaceshipUpgradeEffect(spaceship.Level).ToString()
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

            var upgradeCost = Calculations.getSpaceshipUpgradeCost(spaceship.Level);

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

}
