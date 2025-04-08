using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Endpoints;

//Attack Planet
//Pull necessary data and initiate battle

public static class AttackPlanet
{
    public static IEndpointRouteBuilder AttackPlanetEndpoint(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("api/attack/{planetId:int}", AttackPlanetHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        return endpoint;
    }

    public static async Task<IResult> AttackPlanetHandler(
        [FromRoute] int planetId
        )
    {
        Console.WriteLine(planetId);
        await Task.Delay(1000);
        return Results.BadRequest("Not Implemented");
    }
}
