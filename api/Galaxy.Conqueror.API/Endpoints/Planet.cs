using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Endpoints;

public static class Planet {

    public static IEndpointRouteBuilder PlanetDetails(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/planets", GetAllPlanetDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapGet("api/planet", GetPlanetDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapGet("api/planet/{planetId:int}", GetPlanetDetailsByIdHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();
        return endpoint;
    }

    public static async Task<IResult> GetAllPlanetDetailsHandler(
        [FromServices] UserService userService,
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
            var email = "user1@example.com";
            var user = await userService.GetUserByEmail(email);
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

    public static IEndpointRouteBuilder ResourceExtractors(this IEndpointRouteBuilder endpoint)
    {

        endpoint.MapGet("api/planet/extractor", GetExtractorDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapPut("api/planet/extractor/upgrade", UpgradeExtractorHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }

    public static async Task<IResult> GetExtractorDetailsHandler(
        [FromServices] UserService userService,
        [FromServices] PlanetService planetService,
        [FromServices] ResourceExtractorService resourceExtractorService,
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

            var resourceExtractor = await resourceExtractorService.GetResourceExtractorByUserID(user.Id);
            if (resourceExtractor == null)
                return Results.NotFound("Resource extractor not found.");

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["level"] = resourceExtractor.Level.ToString(),
                ["resourceGen"] = Calculations.GetResourceGenAmount(resourceExtractor.Level).ToString(),
                ["upgradeCost"] = Calculations.GetExtractorUpgradeCost(resourceExtractor.Level).ToString(),
                ["upgradedResourceGen"] = Calculations.GetResourceGenAmount(resourceExtractor.Level + 1).ToString()
            };

            return Results.Ok(dict);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting extractor upgrade details: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> UpgradeExtractorHandler(
        [FromServices] UserService userService,
        [FromServices] SpaceshipService spaceshipService,
        [FromServices] PlanetService planetService,
        [FromServices] ResourceExtractorService resourceExtractorService,
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
                return Results.NotFound("Spaceship not found.");
            
            var planet = await planetService.GetPlanetByUserID(user.Id);

            if (planet == null)
                return Results.NotFound("Planet not found.");

            var resourceExtractor = await resourceExtractorService.GetResourceExtractorByUserID(user.Id);
            if (resourceExtractor == null)
                return Results.NotFound("Resource extractor not found.");

            if (!Calculations.IsInRange(spaceship.X, spaceship.Y, planet.X, planet.Y, 1))
                return Results.BadRequest("Error upgrading resource extractor: Planet out of range");

            var upgradeCost = Calculations.GetExtractorUpgradeCost(resourceExtractor.Level);

            // Validation to check if planet has enough resources for upgrade
            if (planet.ResourceReserve < upgradeCost)
                return Results.BadRequest("Error upgrading resource extractor: Insufficient resources");

            var upgradedExtractor = await resourceExtractorService.UpgradeResourceExtractor(resourceExtractor.Id, planet.Id, upgradeCost);

            return Results.Ok(upgradedExtractor);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error upgrading resource extractor: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static IEndpointRouteBuilder Turrets(this IEndpointRouteBuilder endpoint)
    {

        endpoint.MapGet("api/planet/turret", GetTurretDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapPut("api/planet/turret/upgrade", UpgradeTurretHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }

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
            //var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            //if (string.IsNullOrEmpty(email))
            //    return Results.BadRequest("Email claim not found in token.");
            // testing '
            var email = "user1@example.com";
            var user = await userService.GetUserByEmail(email);
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
            //var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            //if (string.IsNullOrEmpty(email))
            //    return Results.BadRequest("Email claim not found in token.");
            // testing '
            var email = "user1@example.com";
            var user = await userService.GetUserByEmail(email);

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
