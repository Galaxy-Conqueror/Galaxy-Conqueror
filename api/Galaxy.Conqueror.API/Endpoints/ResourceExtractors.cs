using Galaxy.Conqueror.API.Handlers;
namespace Galaxy.Conqueror.API.Endpoints;

public static class ResourceExtractors {

    public static IEndpointRouteBuilder ResourceExtractor(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/extractor", ResourceExtractorHandlers.GetExtractorDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapPut("api/extractor/upgrade", ResourceExtractorHandlers.UpgradeExtractorHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }
}