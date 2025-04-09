using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Menus;
using Galaxy.Conqueror.Client.Models.GameModels;


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
        Console.Clear();
        Console.CursorVisible = false;

        // Input loop
        while (StateManager.state == GameState.RUNNING)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                UserInputHandler.HandleInput(key);
            }


            char[,] gameScreen = null;
            char[,] sidebar = null;

            if (MapView.stale)
            {
                gameScreen = MapView.GetMap();
            }

            if (Sidebar.stale)
            {
                sidebar = Sidebar.GetSidebar();
            }

            Renderer.DrawCanvas(gameScreen, sidebar);

            Thread.Sleep(50);
            
        }
    }
}

