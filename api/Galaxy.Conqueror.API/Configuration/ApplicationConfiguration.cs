using Galaxy.Conqueror.API.Endpoints;

namespace Galaxy.Conqueror.API.Configuration;
public static class ApplicationConfiguration
{
    public static WebApplication ConfigureApp(this WebApplication app)
    {
        // Auth endpoints
        app.LoginEndpoint();
        
        //app.GetUsers();
        app.GetCurrentUser();
        app.UpdateUser();



        app.ViewSpaceshipDetails();
        app.ViewSpaceshipUpgradeDetails();
        app.UpgradeSpaceship();
        app.Refuel();
        app.Repair();
        app.Move();

        app.Battle();

        return app;
    }
}
