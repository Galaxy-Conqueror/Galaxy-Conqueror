using Galaxy.Conqueror.API.Handlers;
using Galaxy.Conqueror.API.Models.Requests;

namespace Galaxy.Conqueror.API.Endpoints;

public static class Auth
{
    public static IEndpointRouteBuilder LoginEndpoint(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("api/auth/login", AuthHandlers.LoginHandler)
            .Accepts<AuthCodeRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        return endpoint;
    }

}