using System.Security.Claims;
using Galaxy.Conqueror.API.Models.Requests;
using Galaxy.Conqueror.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers;

public class UsersHandlers {
    public static async Task<IResult> GetCurrentUserHandler(
        [FromServices] UserService userService,
        HttpContext context,
        CancellationToken ct
    )
    {
        try
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) 
                return Results.BadRequest("Email claim not found in token.");

            var user = await userService.GetUserByEmail(email);
            if (user == null) 
                return Results.NotFound("User not found.");

            return Results.Ok(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user: {ex}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    //public static async Task<IResult> GetUsersHandler(
    //   [FromServices] UserService userService,
    //   CancellationToken ct
    //   )
    //{
    //    var users = await userService.GetUsers();
    //    return Results.Ok(users);
    //}

    public static async Task<IResult> UpdateUserHandler(
        [FromBody] UsernameUpdateRequest request,
        [FromServices] UserService userService,
        CancellationToken ct
    )
    {
        var user = await userService.UpdateUser(new Guid("1bafe6fc-1c69-4377-b9f6-78a07f98d4b1"), request.Username);
        return Results.Ok(user);
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
