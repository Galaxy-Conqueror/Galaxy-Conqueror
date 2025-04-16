using Galaxy.Conqueror.API.Endpoints;

namespace Galaxy.Conqueror.API.Configuration;
public static class ApplicationConfiguration
{
    public static WebApplication ConfigureApp(this WebApplication app)
    {
        app.LoginEndpoint();
        
        app.GetCurrentUser();
        app.UpdateUser();

        app.GetSpaceshipDetails();
        app.UpgradeSpaceship();
        app.Refuel();
        app.Repair();
        app.Deposit();
        app.Move();

        app.PlanetDetails();
        app.ResourceExtractor();
        app.Turret();

        app.Battle();

        return app;
    }
}
