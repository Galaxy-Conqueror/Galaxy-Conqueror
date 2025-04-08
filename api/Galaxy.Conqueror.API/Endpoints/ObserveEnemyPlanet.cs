using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Endpoints;

//At an Enemy Planet
//Observe Planet
//See owner, name, description, (optional: masked stats)


public static class ObserveEnemyPlanet
{
    public static IEndpointRouteBuilder ObserveEnemyPlanetEndpoint(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("api/observe/{planetId:int}", ObserveEnemyPlanetHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        return endpoint;
    }

    public static async Task<IResult> ObserveEnemyPlanetHandler(
        [FromRoute] int planetId
        )
    {
        Console.WriteLine(planetId);
        await Task.Delay(1000);
        return Results.BadRequest("Not Implemented");
    }
}
