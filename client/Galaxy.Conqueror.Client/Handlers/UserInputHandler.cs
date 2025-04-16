using Battle;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Menus;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Linq;

namespace Galaxy.Conqueror.Client.Handlers;

public static class UserInputHandler
{
    private static string inputQueue = "";

    public static void HandleInput()
    {
        ConsoleKey key;

        if (Console.KeyAvailable)
            key = Console.ReadKey(true).Key;
        else
            return;


        var prevPosition = new Vector2I(StateManager.PlayerSpaceship.Position);

        switch (StateManager.State)
        {
            case GameState.MAP_VIEW:
                HandleMapViewInput(key, StateManager.PlayerSpaceship);
                break;

            case GameState.BATTLE:
                HandleBattleInput(key);
                break;

            case GameState.INTRO_VIEW:
                StateManager.State = GameState.MAP_VIEW;
                break;

            default:
                HandleMenuInput(key);
                break;

        }
    }

    private static void HandleMapViewInput(ConsoleKey key, Entity ship)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                MoveShip(ship, 0, -1, '⋀');
                break;

            case ConsoleKey.DownArrow:
                MoveShip(ship, 0, 1, 'V');
                break;

            case ConsoleKey.LeftArrow:
                MoveShip(ship, -1, 0, '<');
                break;

            case ConsoleKey.RightArrow:
                MoveShip(ship, 1, 0, '>');
                break;

            default:
                HandleMenuInput(key);
                break;
        }
    }

    private static void HandleBattleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.LeftArrow:
                BattleEngine.MoveSpaceshipLeft();
                break;
            case ConsoleKey.RightArrow:
                BattleEngine.MoveSpaceshipRight();
                break;

            case ConsoleKey.UpArrow:
                BattleEngine.MoveSpaceshipUp();
                break;
            case ConsoleKey.DownArrow:
                BattleEngine.MoveSpaceshipDown();
                break;

            case ConsoleKey.Spacebar:
                BattleEngine.ShootFromSpaceship();
                break;

            case ConsoleKey.Escape:
                StateManager.State = GameState.MAP_VIEW;
                break;
        }
    }

    private static void MoveShip(Entity ship, int dx, int dy, char directionGlyph)
    {
        ship.Position.X = Math.Clamp(ship.Position.X + dx, 0, StateManager.MAP_WIDTH - 1);
        ship.Position.Y = Math.Clamp(ship.Position.Y + dy, 0, StateManager.MAP_HEIGHT - 1);
        ship.Glyph.Character = directionGlyph;
    }

    private static void HandleMenuInput(ConsoleKey key)
    {
        var menuIndex = (int)key - 'A';
        var menuItems = Sidebar.Content.Items;

        if (menuIndex >= 0 && menuIndex < menuItems.Length)
            menuItems[menuIndex].OnSelect();
    }
}