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
        MapView.InitialiseMap();
        EntityManager.Initialize();

        await Run();
    }

    public static async Task Run()
    {
        Dictionary<Vector2I, char> gameScreen = MapView.GetMap();
        Dictionary<Vector2I, char> sidebar = Sidebar.GetSidebar();

        Console.SetWindowSize(StateManager.CanvasWidth, StateManager.CanvasHeight);
        Console.SetBufferSize(StateManager.CanvasWidth, StateManager.CanvasHeight);

        Console.Clear();
        Console.CursorVisible = false;

        int prevBufferWidth = Console.BufferWidth;
        int prevBufferHeight = Console.BufferHeight;

        Sidebar.stale = true;

        // Input loop
        while (StateManager.State == GameState.RUNNING)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                UserInputHandler.HandleInput(key);
            }




            if (prevBufferWidth != Console.BufferWidth || prevBufferHeight != Console.BufferHeight)
            {
                MapView.stale = true;
            }

            if (MapView.stale)
            {
                gameScreen = MapView.GetMap();
            }

            if (Sidebar.stale)
            {
                Renderer.DrawCanvas(gameScreen, sidebar);
            } 
            else
            {
                Renderer.DrawCanvas(gameScreen, null);
            }

               

            Thread.Sleep(50);
            
        }
    }
}

