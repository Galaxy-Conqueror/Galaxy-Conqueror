using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.GameModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Handlers;

public static class UserInputHandler
{
    private static string inputQueue = "";

    public static void HandleInput(ConsoleKey key)
    {
        var ship = EntityManager.Entities.First(x => x.id == StateManager.playerShipID);

        if (ship == null) return;

        switch (key)
        {
            case ConsoleKey.UpArrow: ship.position.X = Math.Max(0, ship.position.Y - 1); break;
            case ConsoleKey.DownArrow: ship.position.Y = Math.Min(StateManager.MAP_HEIGHT - 1, ship.position.Y + 1); break;
            case ConsoleKey.LeftArrow: ship.position.X = Math.Max(0, ship.position.X - 1); break;
            case ConsoleKey.RightArrow: ship.position.X = Math.Min(ship.position.X + 1, StateManager.MAP_WIDTH - 1); break;
            default:
                {
                    HandleMenuInput(key);
                    break;
                }
        }
    }


    private static void HandleMenuInput(ConsoleKey key)
    {
        StateManager.state = GameState.QUIT_REQUESTED;
        if (key == ConsoleKey.Enter)
        {
            if (inputQueue == "a")
            {
                StateManager.state = GameState.QUIT_REQUESTED;
            }
        }
        else
        {
            inputQueue += key.ToString();
        }
    }
}
