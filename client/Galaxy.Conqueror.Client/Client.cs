using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Menus;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System.Runtime.InteropServices;


namespace Galaxy.Conqueror.Client;

public static class Client
{
    public static async Task Start()
    {
        MapView.Initialise();
        EntityManager.Initialize();

        Console.SetWindowSize(StateManager.CanvasWidth, StateManager.CanvasHeight);
        Console.SetBufferSize(StateManager.CanvasWidth, StateManager.CanvasHeight);

        Console.CursorVisible = false;

        Console.Clear();

        await Run();
    }

    public static async Task Run()
    {
        var prevGameState = GameState.IDLE;

        Dictionary<Vector2I, Glyph> gameScreen = MapView.GetScreen();
        Dictionary<Vector2I, Glyph> sidebar = Sidebar.GetSidebar();

        gameScreen = MapView.GetScreen();
        sidebar = Sidebar.GetSidebar();
        Renderer.DrawCanvas(gameScreen, sidebar, false);

        PlanetView.Initialise();

        while (StateManager.State != GameState.QUIT_REQUESTED)
        {
            // if (StateManager.State == GameState.MAP_VIEW)
            // {
            //     gameScreen = MapView.GetScreen();
            //     sidebar = Sidebar.GetSidebar();
            //     Renderer.DrawCanvas(gameScreen, sidebar, false);
            // }

            // prevGameState = StateManager.State;
        }
    }
}

