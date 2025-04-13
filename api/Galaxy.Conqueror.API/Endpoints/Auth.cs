using Galaxy.Conqueror.API.Models.Requests;
using Galaxy.Conqueror.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace Galaxy.Conqueror.API.Endpoints;

public static class Auth
{
    public static IEndpointRouteBuilder LoginEndpoint(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("api/auth/login", LoginHandler)
            .Accepts<AuthCodeRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        return endpoint;
    }

    public static async Task<IResult> LoginHandler(
        [FromBody] AuthCodeRequest request,
        [FromServices] GoogleAuthService googleAuthService,
        CancellationToken ct
        )
    {
        ct.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(request?.AuthCode);

        var test = new Vector2(0, 0);

        var response = await googleAuthService.Login(request?.AuthCode);

        if (response == null)
        {
            return Results.NoContent();
        }
        return Results.Ok(response);
    }
}