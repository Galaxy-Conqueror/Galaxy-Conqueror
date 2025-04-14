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
    private static int bufferWidth;
    private static int bufferHeight;

    private static Vector2I camera = Vector2I.ZERO;
    private static Vector2I previousCamera = Vector2I.ZERO;

    private static Dictionary<Vector2I, Glyph> previousView = new Dictionary<Vector2I, Glyph>();
    private static Dictionary<Vector2I, Glyph> currentView = new Dictionary<Vector2I, Glyph>();


    private static Dictionary<Vector2I, Glyph> previousSidebar = new();

    private static readonly HashSet<int> visibleEntityIds = new();

    public static bool Stale { get; set; } = true;

    public static void DrawCanvas(Dictionary<Vector2I, Glyph> gameScreen, Dictionary<Vector2I, Glyph>? sidebar, bool staticScreen)
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

            RenderGameScreen(gameScreen, staticScreen);
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

    private static void RenderGameScreen(Dictionary<Vector2I, Glyph> gameScreen, bool staticScreen)
    {
        var cameraOffset = camera;
        var xOffset = StateManager.MAP_SCREEN_WIDTH;
        var yOffset = StateManager.MAP_SCREEN_HEIGHT / 2;

        int minX = cameraOffset.X - (StateManager.MAP_SCREEN_WIDTH / 2);
        int maxX = cameraOffset.X + (StateManager.MAP_SCREEN_WIDTH / 2);
        int minY = cameraOffset.Y - (StateManager.MAP_SCREEN_HEIGHT / 2);
        int maxY = cameraOffset.Y + (StateManager.MAP_SCREEN_HEIGHT / 2);

        if (staticScreen)
        {
            //cameraOffset = Vector2I.ZERO;
            minX = 0;
            maxX = StateManager.MAP_SCREEN_WIDTH;
            minY = 0;
            maxY = StateManager.MAP_SCREEN_HEIGHT;
            xOffset = StateManager.MAP_SCREEN_WIDTH * 2;
            yOffset = StateManager.MAP_SCREEN_HEIGHT;
        }

        foreach (var (position, glyph) in gameScreen)
        {
            if (position.X < minX || position.X > maxX ||
                position.Y < minY || position.Y > maxY)
                continue;

            var canvasX = ((position.X - cameraOffset.X) * 2) + xOffset;
            var canvasY = position.Y - cameraOffset.Y + yOffset;

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
            var canvasY = entity.Position.Y - camera.Y + StateManager.MAP_SCREEN_HEIGHT / 2;

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
}