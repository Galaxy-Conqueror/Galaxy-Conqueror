using Galaxy.Conqueror.API.Endpoints;

namespace Galaxy.Conqueror.API.Configuration;
public static class ApplicationConfiguration
{
    public static WebApplication ConfigureApp(this WebApplication app)
    {
        app.LoginEndpoint();
        return app;
    }
}
