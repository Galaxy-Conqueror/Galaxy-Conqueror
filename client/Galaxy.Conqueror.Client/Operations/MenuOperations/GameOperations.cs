using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.GameModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Operations.MenuOperations;

public static class GameOperations
{
    public static void Pause()
    {
        Console.Clear();
        StateManager.State = GameState.IDLE;
        Console.WriteLine("Paused");
    }

    public static void Resume()
    {
        Console.Clear();
        StateManager.State = GameState.MAP_VIEW;
    }

    public static void Quit()
    {
        StateManager.State = GameState.QUIT_REQUESTED;
    }

    public async static void PingHome()
    {
        await StateManager.PlayerSpaceship.UpdateShipState();
        await StateManager.UpdateOwnPlanet();
        StateManager.CurrentExtractor = await ApiService.GetOwnExtractor();
        StateManager.CurrentTurret = await ApiService.GetOwnTurret();
    }
}
