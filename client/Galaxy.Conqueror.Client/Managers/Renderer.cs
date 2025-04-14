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

    private static Vector2I cameraPosition = Vector2I.ZERO;
    private static Vector2I previousCameraPosition = Vector2I.ZERO;

    private static Dictionary<Vector2I, Glyph> previousMap = new Dictionary<Vector2I, Glyph>();
    private static Dictionary<Vector2I, Glyph> currentMap = new Dictionary<Vector2I, Glyph>();

    private static Dictionary<Vector2I, Glyph> currentSidebar = new Dictionary<Vector2I, Glyph>();
    private static Dictionary<Vector2I, Glyph> previousSidebar = new Dictionary<Vector2I, Glyph>();

    private static readonly HashSet<int> visibleEntityIds = new();

    public static bool Stale { get; set; } = true;

    public static void RenderMap()
    {
        if (bufferWidth != Console.BufferWidth || bufferHeight != Console.BufferHeight)
        {
            Console.Clear();
            bufferWidth = Console.BufferWidth;
            bufferHeight = Console.BufferHeight;
            previousMap.Clear();
            currentMap.Clear();
        }

        var playerShip = EntityManager.Entities.FirstOrDefault(x => x.Id == StateManager.PlayerShipID);

        if (playerShip == null) return;

        previousCameraPosition = new Vector2I(cameraPosition.X, cameraPosition.Y);
        cameraPosition = new Vector2I(playerShip.Position.X, playerShip.Position.Y);

        bool cameraChanged = !cameraPosition.Equals(previousCameraPosition);

        if (!cameraChanged) return;

        int MAP_WIDTH = StateManager.MAP_SCREEN_WIDTH;
        int MAP_HEIGHT = StateManager.MAP_SCREEN_HEIGHT;

        int minX = cameraPosition.X - (MAP_WIDTH / 2);
        int maxX = cameraPosition.X + (MAP_WIDTH / 2);
        int minY = cameraPosition.Y - (MAP_HEIGHT / 2);
        int maxY = cameraPosition.Y + (MAP_HEIGHT / 2);

        foreach (var (position, glyph) in MapView.GetScreen())
        {
            if (position.X < minX || position.X > maxX ||
                position.Y < minY || position.Y > maxY)
                continue;

            var canvasX = ((position.X - cameraPosition.X) * 2) + StateManager.MAP_SCREEN_WIDTH;
            var canvasY = position.Y - cameraPosition.Y + StateManager.MAP_SCREEN_HEIGHT / 2;

            if (IsInCanvas(canvasX, canvasY))
            {
                Vector2I pos = new Vector2I(canvasX, canvasY);
                currentMap.Add(pos, glyph);
                previousMap.Remove(pos);
            }
        }

        foreach (var entity in EntityManager.Entities)
        {
            if (entity.Position.X < minX || entity.Position.X > maxX ||
                entity.Position.Y < minY || entity.Position.Y > maxY)
                continue;

            var canvasX = ((entity.Position.X - cameraPosition.X) * 2) + StateManager.MAP_SCREEN_WIDTH;
            var canvasY = entity.Position.Y - cameraPosition.Y + StateManager.MAP_SCREEN_HEIGHT / 2;

            if (IsInCanvas(canvasX, canvasY))
            {
                Vector2I pos = new Vector2I(canvasX, canvasY);

                currentMap.Remove(pos);
                currentMap.Add(pos, entity.Glyph);
                previousMap.Remove(pos);

                visibleEntityIds.Add(entity.Id);

                EntityManager.PrevEntityPositions[entity.Id] = entity.Position;
            }
        }

        foreach (var (position, glyph) in currentMap)
        {
            Console.SetCursorPosition(position.X, position.Y);
            ConsolePrinter.PrintGlyph(glyph);
        }

        ClearMap();
    }

    private static void ClearMap()
    {
        foreach (var (position, glyph) in previousMap)
        {
            ConsolePrinter.ClearGlyph(position);
        }

        previousMap = new Dictionary<Vector2I, Glyph>(currentMap);
        currentMap.Clear();
    }

    public static void RenderSidebar()
    {
        Dictionary<Vector2I, Glyph> sidebar = Sidebar.GetSidebar();

        int minX = 0;
        int maxX = StateManager.MAP_SCREEN_WIDTH;
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
                Vector2I pos = new Vector2I(canvasX, canvasY);

                currentSidebar.Add(pos, glyph);
                previousSidebar.Remove(pos);

                Console.SetCursorPosition(canvasX, canvasY);
                ConsolePrinter.PrintGlyph(glyph);
            }
        }

        ClearSidebar();
    }

    private static void ClearSidebar()
    {
        foreach (var (position, glyph) in previousSidebar)
        {
            ConsolePrinter.ClearGlyph(position);
        }

        previousSidebar = new Dictionary<Vector2I, Glyph>(currentSidebar);
        currentSidebar.Clear();
    }

    public static bool IsInCanvas(int x, int y)
    {
        return x >= 0 && x < Console.BufferWidth && y >= 0 && y < Console.BufferHeight;
    }
}