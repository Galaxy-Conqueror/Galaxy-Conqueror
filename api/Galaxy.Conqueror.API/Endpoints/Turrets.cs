using Galaxy.Conqueror.API.Handlers;
using Galaxy.Conqueror.API.Models.Responses;
namespace Galaxy.Conqueror.API.Endpoints;

public static class Turrets {

    public static IEndpointRouteBuilder Turret(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/turret", TurretsHandlers.GetTurretDetailsHandler)
            .Produces<TurretDetailResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        endpoint.MapPut("api/turret/upgrade", TurretsHandlers.UpgradeTurretHandler)
            .Produces<TurretUpgradedResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        return endpoint;
    }
}