using Galaxy.Conqueror.API.Handlers;
using Galaxy.Conqueror.API.Models;
using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Endpoints;

public static class Battles
{
    public static IEndpointRouteBuilder Battle(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/battle/{planetId:int}", BattleHandlers.BattleHandler)
            .Produces<BattleResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapPost("api/battle/{planetId:int}/log", BattleHandlers.BattleLogHandler)
            .Produces<Battle>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }

    
}
