using Galaxy.Conqueror.API.Models.Requests;
using Galaxy.Conqueror.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Galaxy.Conqueror.API.Handlers;

public class AuthHandlers {
    public static async Task<IResult> LoginHandler(
        [FromBody] AuthCodeRequest request,
        [FromServices] GoogleAuthService googleAuthService,
        CancellationToken ct
        )
    {
        ct.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(request?.AuthCode);

        var response = await googleAuthService.Login(request?.AuthCode);

        if (response == null)
        {
            return Results.NoContent();
        }
        return Results.Ok(response);

    }
}
