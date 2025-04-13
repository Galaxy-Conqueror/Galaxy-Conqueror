using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Galaxy.Conqueror.Client.Menus;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;

namespace Galaxy.Conqueror.Client.Managers;

public static class Renderer
{
    private static readonly Dictionary<Vector2I, char> canvas = new();

    private static int bufferWidth;
    private static int bufferHeight;

    private static Vector2I camera = Vector2I.ZERO;
    private static Vector2I previousCamera = Vector2I.ZERO;

    private static Dictionary<Vector2I, Glyph> previousView = new Dictionary<Vector2I, Glyph>();
    private static Dictionary<Vector2I, Glyph> currentView = new Dictionary<Vector2I, Glyph>();


    private static Dictionary<Vector2I, Glyph> previousSidebar = new();

    private static readonly HashSet<int> visibleEntityIds = new();

    public static bool Stale { get; set; } = true;

    public static void DrawCanvas(Dictionary<Vector2I, Glyph> gameScreen, Dictionary<Vector2I, Glyph>? sidebar)
    {
        bufferWidth = Console.BufferWidth;
        bufferHeight = Console.BufferHeight;

        var playerShip = EntityManager.Entities.FirstOrDefault(x => x.Id == StateManager.PlayerShipID);
        if (playerShip == null) return;

        previousCamera = camera;
        camera = playerShip.Position;
        bool cameraChanged = !camera.Equals(previousCamera);

        if (Stale || cameraChanged)
        {
            if (sidebar != null &&
                (!sidebar.SequenceEqual(previousSidebar ?? new Dictionary<Vector2I, Glyph>())))
            {
                RenderSidebar(sidebar);
                previousSidebar = new Dictionary<Vector2I, Glyph>(sidebar);
            }

            RenderGameScreen(gameScreen);
            RenderEntities();
            ClearPreviousCanvas();

            Stale = false;
        }
    }

    private static void ClearPreviousCanvas()
    {
        foreach (var (position, glyph) in previousView)
        {
            ConsolePrinter.ClearGlyph(position);
        }

        previousView = new Dictionary<Vector2I, Glyph>(currentView);
        currentView.Clear();
    }

    private static void RenderGameScreen(Dictionary<Vector2I, Glyph> gameScreen)
    {
        int minX = camera.X - (StateManager.MAP_SCREEN_WIDTH / 2);
        int maxX = camera.X + (StateManager.MAP_SCREEN_WIDTH / 2);
        int minY = camera.Y - (StateManager.MAP_SCREEN_HEIGHT / 2);
        int maxY = camera.Y + (StateManager.MAP_SCREEN_HEIGHT / 2);

        foreach (var (position, glyph) in gameScreen)
        {
            if (position.X < minX || position.X > maxX ||
                position.Y < minY || position.Y > maxY)
                continue;

            var canvasX = ((position.X - camera.X) * 2) + StateManager.MAP_SCREEN_WIDTH;
            var canvasY = position.Y - camera.Y + StateManager.MAP_SCREEN_HEIGHT / 4;

            if (IsInCanvas(canvasX, canvasY))
            {
                Vector2I pos = new Vector2I(canvasX, canvasY);
                currentView.Add(pos, glyph);
                previousView.Remove(pos);
                Console.SetCursorPosition(canvasX, canvasY);
                ConsolePrinter.PrintGlyph(glyph);
            }
        }
    }

    private static void RenderSidebar(Dictionary<Vector2I, Glyph> sidebar)
    {
        int minX = 0;
        int maxX = StateManager.MAP_WIDTH;
        int minY = 0;
        int maxY = StateManager.MAP_SCREEN_HEIGHT;

        foreach (var (position, glyph) in sidebar)
        {
            if (position.X < minX || position.X > maxX ||
                position.Y < minY || position.Y > maxY)
                continue;

            var canvasX = position.X + StateManager.MAP_SCREEN_WIDTH * 2 + StateManager.MENU_MARGIN;
            var canvasY = position.Y;

            if (IsInCanvas(canvasX, canvasY))
            {
                Console.SetCursorPosition(canvasX, canvasY);
                ConsolePrinter.PrintGlyph(glyph);
            }
        }
    }

    private static void RenderEntities()
    {
        visibleEntityIds.Clear();

        int minX = camera.X - (StateManager.MAP_SCREEN_WIDTH / 2);
        int maxX = camera.X + (StateManager.MAP_SCREEN_WIDTH / 2);
        int minY = camera.Y - (StateManager.MAP_SCREEN_HEIGHT / 2);
        int maxY = camera.Y + (StateManager.MAP_SCREEN_HEIGHT / 2);

        foreach (var entity in EntityManager.Entities)
        {
            if (entity.Position.X < minX || entity.Position.X > maxX ||
                entity.Position.Y < minY || entity.Position.Y > maxY)
                continue;

            var canvasX = ((entity.Position.X - camera.X) * 2) + StateManager.MAP_SCREEN_WIDTH;
            var canvasY = entity.Position.Y - camera.Y + StateManager.MAP_SCREEN_HEIGHT / 4;

            if (IsInCanvas(canvasX, canvasY))
            {
                Vector2I pos = new Vector2I(canvasX, canvasY);

                currentView.Remove(pos);
                currentView.Add(pos, entity.Glyph);
                previousView.Remove(pos);

                Console.SetCursorPosition(canvasX, canvasY);
                ConsolePrinter.PrintGlyph(entity.Glyph);

                visibleEntityIds.Add(entity.Id);

                EntityManager.PrevEntityPositions[entity.Id] = entity.Position;
                entity.Stale = false;
            }
        }
    }

    public static bool IsInCanvas(int x, int y)
    {
        return x >= 0 && x < bufferWidth && y >= 0 && y < bufferHeight;
    }

    public static bool IsInCanvas(Vector2I position)
    {
        return IsInCanvas(position.X, position.Y);
    }
}