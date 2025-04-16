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
    static GameState prevGameState = GameState.INTRO_VIEW;

    public static async Task Start()
    {
        StateManager.State = GameState.INTRO_VIEW;
        await AuthHelper.Authenticate();

        MapView.Initialise();
        await EntityManager.Initialize();
        await StateManager.PlayerSpaceship.UpdateShipState();
        Sidebar.MockMenu();

        if (StateManager.PlayerPlanet.Name == "") {
            await AuthHelper.SetPlanetName();
        }

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
                    await Renderer.RenderSidebar();
                    break;

                case GameState.BATTLE:
                    if (!BattleEngine.GameRunning)
                    {
                       
                    }
                    else
                    {
                        BattleEngine.Update();
                        Renderer.RenderBattleMap();
                    }                
                    break;

                case GameState.PLANET_VIEW:
                    Renderer.RenderImage();
                    await Renderer.RenderSidebar();
                    break;

                case GameState.INTRO_VIEW:
                    Renderer.RenderImage();
                    await Renderer.RenderSidebar();
                    break;

                default:
                    break;
            }

            if (stateHasChanged() && StateManager.State != GameState.PLANET_VIEW)
            {
                Renderer.ReRender = true;
                Console.Clear();
            }

            prevGameState = StateManager.State;
        }
    }

    private static bool stateHasChanged()
    {
        return StateManager.State != prevGameState;
    }
}

