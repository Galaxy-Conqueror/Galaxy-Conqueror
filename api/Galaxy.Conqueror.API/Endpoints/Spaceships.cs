using Galaxy.Conqueror.API.Handlers.Spaceships;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Models.Responses;

namespace Galaxy.Conqueror.API.Endpoints;

public static class Spaceships
{
    public static IEndpointRouteBuilder GetSpaceshipDetails(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship", DetailsHandlers.GetSpaceshipDetailsHandler)
            .Produces<SpaceshipResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();
        return endpoint;
    }

    public static IEndpointRouteBuilder UpgradeSpaceship(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship/upgrade", UpgradeHandlers.GetSpaceshipUpgradeDetailsHandler)
            .Produces<SpaceshipUpgradeResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

            endpoint.MapPut("api/spaceship/upgrade", UpgradeHandlers.UpgradeSpaceshipHandler)
            .Produces<SpaceshipUpgradedResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }

    public static IEndpointRouteBuilder Refuel(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship/refuel", RefuelHandlers.GetRefuelPriceHandler)
            .Produces<int>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapPut("api/spaceship/refuel", RefuelHandlers.RefuelSpaceshipHandler)
            .Produces<SpaceshipRefuelResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }

    public static IEndpointRouteBuilder Repair(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/spaceship/repair", RepairHandlers.RepairPriceHandler)
            .Produces<int>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        endpoint.MapPut("api/spaceship/repair", RepairHandlers.RepairSpaceshipHandler)
            .Produces<SpaceshipRepairResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;

    }  

    public static IEndpointRouteBuilder Move(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPut("api/spaceship/move", MoveHandlers.MoveSpaceshipHandler)
            .Produces<Spaceship>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;
    }

    public static IEndpointRouteBuilder Deposit(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPut("api/spaceship/deposit", DepositHandlers.DepositResourcesHandler)
            .Produces<Planet>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            //.RequireAuthorization();

        return endpoint;

    }
    

}
