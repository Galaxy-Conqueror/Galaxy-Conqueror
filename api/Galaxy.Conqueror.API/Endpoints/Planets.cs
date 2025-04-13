using Galaxy.Conqueror.API.Handlers;
using Galaxy.Conqueror.API.Models.Requests;
namespace Galaxy.Conqueror.API.Endpoints;

public static class PlanetsEndpoints {

    public static IEndpointRouteBuilder PlanetDetails(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/planets", PlanetsHandlers.GetAllPlanetDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapGet("api/planet", PlanetsHandlers.GetPlanetDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapGet("api/planet/{planetId:int}", PlanetsHandlers.GetPlanetDetailsByIdHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        //.RequireAuthorization();

        endpoint.MapPut("api/planet", PlanetsHandlers.UpdatePlanetNameHandler)
            .Accepts<PlanetNameRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        //.RequireAuthorization();

        return endpoint;
    }
}