﻿using Battle;
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
        StateManager.CurrentExtractor = await ApiService.GetOwnExtractor();
        StateManager.CurrentTurret = await ApiService.GetOwnTurret();
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

        var running = true;

        while (running)
        {
            UserInputHandler.HandleInput();

            switch (StateManager.State)
            {
                case GameState.MAP_VIEW:
                    Renderer.RenderMap();
                    await Renderer.RenderSidebar();
                    break;

                case GameState.BATTLE:
                    BattleEngine.Update();

                    if (BattleEngine.GameRunning)
                        Renderer.RenderBattleMap();
                                  
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

            if (StateManager.State == GameState.QUIT_REQUESTED)
            {
                Console.Clear();
                Console.Write("Quit? (Y/N): ");
                
                while (running && StateManager.State != prevGameState)
                {
                    var response = Console.ReadLine();

                    if (response?.ToUpper() == "Y")
                    {
                        running = false;
                    }
                    else
                    {
                        Console.Clear();
                        StateManager.State = prevGameState;
                    }
                }
            }

            prevGameState = StateManager.State;
        }


    }

    private static bool stateHasChanged()
    {
        return StateManager.State != prevGameState;
    }
}

