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

            if (prevGameState == GameState.MAP_VIEW && prevGameState != GameState.MAP_VIEW)
            {
                MapView.stale = true;

            }

            if (StateManager.State == GameState.MAP_VIEW)
            {
                if (prevBufferWidth != Console.BufferWidth || prevBufferHeight != Console.BufferHeight)
                {
                    MapView.stale = true;
                }

                if (MapView.stale)
                {
                    gameScreen = MapView.GetScreen();
                }
            }
            else if (StateManager.State == GameState.PLANET_MANAGEMENT && PlanetView.Stale)
            {
                Renderer.Stale = true;
                // gameScreen = PlanetView.GetScreen();
            }


            Sidebar.CheckSidebarState();

            if (Sidebar.stale)
            {
                sidebar = Sidebar.GetSidebar();
            }

            if (StateManager.State == GameState.PLANET_MANAGEMENT && prevGameState != GameState.PLANET_MANAGEMENT)
            {
                Renderer.DrawCanvas(gameScreen, null);
            }

            Renderer.DrawCanvas(gameScreen, sidebar);
        }
    }
}

