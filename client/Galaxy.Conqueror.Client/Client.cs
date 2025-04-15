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

        await AuthHelper.Authenticate();

        MapView.Initialise();
        await EntityManager.Initialize();
        Sidebar.MockMenu();

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
                    Renderer.RenderSidebar();
                    break;

                case GameState.BATTLE:
                    if (!BattleEngine.gameRunning)
                    {
                        Spaceship spaceship = new Spaceship(1, "Player", new Glyph('⋀', ConsoleColor.Yellow), new Vector2I(0, 0), "");
                        spaceship.Level = 400;
                        spaceship.CurrentHealth = 100;
                        Turret turret = new Turret(2, "Enemy", new Glyph('V', ConsoleColor.Red), new Vector2I(0, 0));
                        turret.Level = 600;
                        turret.CurrentHealth = 100;
                        BattleEngine.Initialise(StateManager.MAP_SCREEN_WIDTH, StateManager.MAP_SCREEN_HEIGHT, spaceship, turret);

                        BattleEngine.OnBattleConcluded(battleResult =>
                        {
                            Console.Clear();
                            Console.WriteLine($"Battle over! Winner: {battleResult.WinnerName}");
                            Console.WriteLine($"Spaceship HP: {battleResult.SpaceshipHealth}, Turret HP: {battleResult.TurretHealth}");
                            Console.WriteLine($"Battle lasted {battleResult.BattleDurationSeconds:F2} seconds");
                            StateManager.State = GameState.MAP_VIEW;
                            Thread.Sleep(2000);
                        });
                    }
                    else
                    {
                        BattleEngine.Update();
                        Renderer.RenderBattleMap();
                    }                
                    break;

                case GameState.PLANET_MANAGEMENT:
                    Renderer.RenderImage();
                    Renderer.RenderSidebar();
                    break;

                default:
                    break;
            }

            if (stateHasChanged() && StateManager.State != GameState.PLANET_MANAGEMENT) Console.Clear();

            prevGameState = StateManager.State;
        }
    }

    private static bool stateHasChanged()
    {
        return StateManager.State != prevGameState;
    }
}

