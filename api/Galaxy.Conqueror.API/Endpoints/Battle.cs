using Galaxy.Conqueror.API.Handlers;

namespace Galaxy.Conqueror.API.Endpoints;

public static class Battles
{
    public static IEndpointRouteBuilder Battle(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/battle/{planetId:int}", BattleHandlers.BattleHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapPost("api/battle/{planetId:int}/log", BattleHandlers.BattleLogHandler)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }

    
}
