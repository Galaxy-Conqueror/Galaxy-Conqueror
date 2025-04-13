using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Galaxy.Conqueror.API.Configuration.Database;

namespace Galaxy.Conqueror.API.Endpoints;

public static class DbPing
{
    public static IEndpointRouteBuilder AddDbPingEndpoint(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/db-ping", async ([FromServices] IDbConnectionFactory dbFactory) =>
        {
            try
            {
                using var connection = dbFactory.CreateConnection();
                var result = await connection.ExecuteScalarAsync<DateTime>("SELECT NOW()");
                return Results.Ok(new { message = $"DB Time: {result}" });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Failed to connect to DB: {ex.Message}");
            }
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        return endpoint;
    }
}
