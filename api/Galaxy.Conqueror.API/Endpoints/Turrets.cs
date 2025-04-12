using Galaxy.Conqueror.API.Handlers;
namespace Galaxy.Conqueror.API.Endpoints;

public static class Turrets {

    public static IEndpointRouteBuilder Turret(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/turret", TurretsHandlers.GetTurretDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapPut("api/turret/upgrade", TurretsHandlers.UpgradeTurretHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }
}