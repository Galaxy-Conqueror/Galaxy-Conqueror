using Galaxy.Conqueror.API.Handlers.Spaceships;

namespace Galaxy.Conqueror.API.Endpoints;

public static class Spaceships
{
    public static IEndpointRouteBuilder GetSpaceshipDetails(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship", DetailsHandlers.GetSpaceshipDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();
        return endpoint;
    }

    public static IEndpointRouteBuilder UpgradeSpaceship(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship/upgrade", UpgradeHandlers.GetSpaceshipUpgradeDetailsHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

            endpoint.MapPost("api/spaceship/upgrade", UpgradeHandlers.UpgradeSpaceshipHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }

    public static IEndpointRouteBuilder Refuel(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship/refuel", RefuelHandlers.GetRefuelPriceHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapPut("api/spaceship/refuel", RefuelHandlers.RefuelSpaceshipHandler)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);
        //.RequireAuthorization();

        return endpoint;

    }

    public static IEndpointRouteBuilder Repair(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship/repair", RepairHandlers.RepairPriceHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapPut("api/spaceship/repair", RepairHandlers.RepairSpaceshipHandler)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;

    }  

    public static IEndpointRouteBuilder Move(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPut("api/spaceship/move", MoveHandlers.MoveSpaceshipHandler)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }

    public static IEndpointRouteBuilder Deposit(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPut("api/spaceship/deposit", DepositHandlers.DepositResourcesHandler)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;

    }
    

}
