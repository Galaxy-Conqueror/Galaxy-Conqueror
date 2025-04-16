using Galaxy.Conqueror.API.Handlers;
using Galaxy.Conqueror.API.Models.Responses;
namespace Galaxy.Conqueror.API.Endpoints;

public static class ResourceExtractors {

    public static IEndpointRouteBuilder ResourceExtractor(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/extractor", ResourceExtractorHandlers.GetExtractorDetailsHandler)
            .Produces<ResourceExtractorDetailResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        endpoint.MapPut("api/extractor/upgrade", ResourceExtractorHandlers.UpgradeExtractorHandler)
            .Produces<ResourceExtractorUpgradedResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        return endpoint;
    }
}