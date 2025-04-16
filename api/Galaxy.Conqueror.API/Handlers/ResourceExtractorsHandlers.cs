using Galaxy.Conqueror.API.Models;
using Galaxy.Conqueror.API.Models.Responses;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers;

public class ResourceExtractorHandlers {
    
    public static async Task<IResult> GetExtractorDetailsHandler(
        [FromServices] IUserService userService,
        [FromServices] ResourceExtractorService resourceExtractorService,
        HttpContext context
    )
    {
        try
        {
            var user = await userService.GetUserByContext(context);
            if (user == null)
                return Results.NotFound("User not found.");

            var resourceExtractor = await resourceExtractorService.GetResourceExtractorByUserID(user.Id);
            if (resourceExtractor == null)
                return Results.NotFound("Resource extractor not found.");

            ResourceExtractorDetailResponse response = new()
            {
                Id = resourceExtractor.Id,
                PlanetId = resourceExtractor.PlanetId,
                Level = resourceExtractor.Level,
                ResourceGen = Calculations.GetResourceGenAmount(resourceExtractor.Level),
                UpgradedResourceGen = Calculations.GetResourceGenAmount(resourceExtractor.Level + 1),
                UpgradeCost = Calculations.GetExtractorUpgradeCost(resourceExtractor.Level)
            };

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting extractor upgrade details: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public static async Task<IResult> UpgradeExtractorHandler(
        [FromServices] IUserService userService,
        [FromServices] SpaceshipService spaceshipService,
        [FromServices] PlanetService planetService,
        [FromServices] ResourceExtractorService resourceExtractorService,
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

            var resourceExtractor = await resourceExtractorService.GetResourceExtractorByUserID(user.Id);
            if (resourceExtractor == null)
                return Results.NotFound("Resource extractor not found.");

            if (!Calculations.IsInRange(spaceship.X, spaceship.Y, planet.X, planet.Y, 1))
                return Results.BadRequest("Error upgrading resource extractor: Planet out of range");

            var upgradeCost = Calculations.GetExtractorUpgradeCost(resourceExtractor.Level);
            if (planet.ResourceReserve < upgradeCost)
                return Results.BadRequest("Error upgrading resource extractor: Insufficient resources");

            var upgradedExtractor = await resourceExtractorService.UpgradeResourceExtractor(resourceExtractor.Id, planet.Id, upgradeCost);

            ResourceExtractorUpgradedResponse response = new()
            {
                Level = upgradedExtractor.Level,
                ResourceGen = Calculations.GetResourceGenAmount(upgradedExtractor.Level),
                PlanetResourceReserve = planet.ResourceReserve - upgradeCost
            };

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error upgrading resource extractor: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
