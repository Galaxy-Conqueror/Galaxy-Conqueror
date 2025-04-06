namespace Galaxy.Conqueror.API.Endpoints;

using Galaxy.Conqueror.API.Models.Requests;
using Galaxy.Conqueror.API.Services;
using Microsoft.AspNetCore.Mvc;

public static class User
{
    public static IEndpointRouteBuilder GetUserById(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/user", GetUserByIdHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();
        return endpoint;
    }

    public static async Task<IResult> GetUserByIdHandler(
        //[FromRoute] Guid userId,
        [FromServices] UserService userService,
        HttpContext context,
        CancellationToken ct
    )
    {
        var user2 = context.User;
        var user = await userService.GetUserById(new Guid("1bafe6fc-1c69-4377-b9f6-78a07f98d4b1"));
        return Results.Ok(user);
    }


    //public static IEndpointRouteBuilder GetUsers(this IEndpointRouteBuilder endpoint)
    //{
    //    endpoint.MapGet("api/user", GetUsersHandler)
    //        .Produces(StatusCodes.Status200OK)
    //        .Produces(StatusCodes.Status400BadRequest)
    //        .Produces(StatusCodes.Status500InternalServerError);
    //    return endpoint;
    //}
    //public static async Task<IResult> GetUsersHandler(
    //   [FromServices] UserService userService,
    //   CancellationToken ct
    //   )
    //{
    //    var users = await userService.GetUsers();
    //    return Results.Ok(users);
    //}

    public static IEndpointRouteBuilder UpdateUser(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPatch("api/user", UpdateUserHandler)
            .Accepts<UsernameUpdateRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        return endpoint;
    }

    public static async Task<IResult> UpdateUserHandler(
        [FromBody] UsernameUpdateRequest request,
        [FromServices] UserService userService,
        CancellationToken ct
    )
    {
        var user = await userService.UpdateUser(new Guid("1bafe6fc-1c69-4377-b9f6-78a07f98d4b1"), request.Username);
        return Results.Ok(user);
    }

    public static IEndpointRouteBuilder DeleteUser(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapDelete("api/user", DeleteUserHandler)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        return endpoint;
    }

    public static async Task<IResult> DeleteUserHandler(
        [FromServices] UserService userService,
        CancellationToken ct
    )
    {
        var userId = "1bafe6fc-1c69-4377-b9f6-78a07f98d4b1";
        try
        {
            
            await userService.DeleteUser(userId);
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            //logger.LogError(ex, "Error deleting user with ID {UserId}", userId);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
        
    }

}
