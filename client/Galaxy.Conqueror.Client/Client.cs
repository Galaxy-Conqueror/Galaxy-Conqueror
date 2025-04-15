using Battle;
using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Menus;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System.Runtime.InteropServices;


namespace Galaxy.Conqueror.Client;

public static class Client
{
    static GameState prevGameState = GameState.IDLE;

    public static async Task Start()
    {

        // await AuthHelper.Authenticate();

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
        PlanetView.Initialise();

        while (StateManager.State != GameState.QUIT_REQUESTED)
        {
            UserInputHandler.HandleInput();

            switch (StateManager.State)
            {
                case GameState.MAP_VIEW:
                    Renderer.RenderMap();
                    // Renderer.RenderSidebar(); // adding this makes everything render weirdly when you go close to planets
                    break;

                case GameState.BATTLE:
                    BattleEngine.Update();
                    Renderer.RenderBattleMap();
                    break;

                default:
                    break;
            }

            if (stateHasChanged()) Console.Clear();

            prevGameState = StateManager.State;
        }
    }

    private static bool stateHasChanged()
    {
        return StateManager.State != prevGameState;
    }
}

