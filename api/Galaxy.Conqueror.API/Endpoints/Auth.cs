using Galaxy.Conqueror.API.Handlers;
using Galaxy.Conqueror.API.Models.Requests;
using Galaxy.Conqueror.API.Models.Responses;

namespace Galaxy.Conqueror.API.Endpoints;

public static class Auth
{
    public static IEndpointRouteBuilder LoginEndpoint(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("api/auth/login", AuthHandlers.LoginHandler)
            .Accepts<AuthCodeRequest>("application/json")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        return endpoint;
    }
}