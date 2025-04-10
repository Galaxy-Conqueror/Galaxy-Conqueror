﻿using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
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
        var ship = EntityManager.Entities.First(x => x.Id == StateManager.PlayerShipID);

        var prevPosition = new Vector2I(ship.Position);

        if (ship == null) return;

        switch (key)
        {
            case ConsoleKey.UpArrow: ship.Position.Y = Math.Max(0, ship.Position.Y - 1); break;
            case ConsoleKey.DownArrow: ship.Position.Y = Math.Min(StateManager.MAP_HEIGHT - 1, ship.Position.Y + 1); break;
            case ConsoleKey.LeftArrow: ship.Position.X = Math.Max(0, ship.Position.X - 1); break;
            case ConsoleKey.RightArrow: ship.Position.X = Math.Min(ship.Position.X + 1, StateManager.MAP_WIDTH - 1); break;
            default:
                {
                    HandleMenuInput(key);
                    break;
                }
        }

        if (prevPosition != ship.Position)
        {
            Renderer.Stale = true;
        }
    }


    private static void HandleMenuInput(ConsoleKey key)
    {
        StateManager.State = GameState.QUIT_REQUESTED;
        if (key == ConsoleKey.Enter)
        {
            if (inputQueue == "a")
            {
                StateManager.State = GameState.QUIT_REQUESTED;
            }
        }
        else
        {
            inputQueue += key.ToString();
        }
    }
}
