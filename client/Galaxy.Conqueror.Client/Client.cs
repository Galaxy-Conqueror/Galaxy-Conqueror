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

        await Run();
    }

    public static async Task Run()
    {
        Dictionary<Vector2I, Glyph> gameScreen = MapView.GetScreen();
        Dictionary<Vector2I, Glyph> sidebar = Sidebar.GetSidebar();

        PlanetView.Initialise();

        var prevGameState = GameState.IDLE;

        Console.Clear();
        Console.CursorVisible = false;

        Console.Clear();

        int prevBufferWidth = Console.BufferWidth;
        int prevBufferHeight = Console.BufferHeight;

        Sidebar.stale = true;

        // Input loop
        while (StateManager.State != GameState.QUIT_REQUESTED)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                UserInputHandler.HandleInput(key);
            }

            if (StateManager.State == GameState.MAP_VIEW)
            {
                Renderer.RenderMap();
            }
            else if (StateManager.State == GameState.PLANET_MANAGEMENT && PlanetView.Stale)
            {
                // gameScreen = PlanetView.GetScreen();
            }

            Sidebar.UpdateSidebarState();

            if (Sidebar.stale)
            {
                Renderer.RenderSidebar();
            }

            if (StateManager.State == GameState.PLANET_MANAGEMENT && prevGameState != GameState.PLANET_MANAGEMENT)
            {
                // Renderer.DrawCanvas(gameScreen, null);
            }
        }
    }
}

