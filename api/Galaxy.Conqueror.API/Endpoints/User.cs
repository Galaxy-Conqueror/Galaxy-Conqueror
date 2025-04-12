namespace Galaxy.Conqueror.API.Endpoints;

using Galaxy.Conqueror.API.Handlers;
using Galaxy.Conqueror.API.Models.Requests;
using Microsoft.AspNetCore.Builder;

public static class User
{
    public static IEndpointRouteBuilder GetCurrentUser(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/user", UsersHandlers.GetCurrentUserHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();
        return endpoint;
    }

    //public static IEndpointRouteBuilder GetUsers(this IEndpointRouteBuilder endpoint)
    //{
    //    endpoint.MapGet("api/user", UsersHandlers.GetUsersHandler)
    //        .Produces(StatusCodes.Status200OK)
    //        .Produces(StatusCodes.Status400BadRequest)
    //        .Produces(StatusCodes.Status500InternalServerError);
    //    return endpoint;
    //}

    public static IEndpointRouteBuilder UpdateUser(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPatch("api/user", UsersHandlers.UpdateUserHandler)
            .Accepts<UsernameUpdateRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        return endpoint;
    }

    public static IEndpointRouteBuilder DeleteUser(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapDelete("api/user", UsersHandlers.DeleteUserHandler)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        return endpoint;
    }

    

}
