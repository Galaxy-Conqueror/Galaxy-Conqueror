using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Battle;
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

    private static Dictionary<Vector2I, Glyph> currentImage = new Dictionary<Vector2I, Glyph>();
    private static Dictionary<Vector2I, Glyph> previousImage = new Dictionary<Vector2I, Glyph>();
    private static bool imageRendered { get; set; } = false;

    private static Dictionary<Vector2I, Glyph> previousBattleMap = new();
    private static Vector2I previousSpaceship = Vector2I.ZERO;

    private static readonly HashSet<int> visibleEntityIds = new();

    public static bool ReRender { get; set; } = true;

    public static void ClearAll()
    {
        Console.Clear();
    }

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

        if (StateManager.PlayerSpaceship == null) return;

        previousCameraPosition = new Vector2I(cameraPosition.X, cameraPosition.Y);
        cameraPosition = new Vector2I(StateManager.PlayerSpaceship.Position.X, StateManager.PlayerSpaceship.Position.Y);

        bool cameraChanged = !cameraPosition.Equals(previousCameraPosition);

        if (!cameraChanged && !ReRender) return;

        ReRender = false;

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
            Console.SetCursorPosition(position.X, position.Y);
            ConsolePrinter.ClearGlyph();
        }

        previousMap = new Dictionary<Vector2I, Glyph>(currentMap);
        currentMap.Clear();
    }

    public async static Task RenderSidebar()
    {
        await Sidebar.CheckSidebarState();
        Dictionary<Vector2I, Glyph> sidebar = Sidebar.GetSidebar();
        var items = Sidebar.Content.Items;

        int minX = StateManager.MAP_SCREEN_WIDTH;
        int maxX = (StateManager.MAP_SCREEN_WIDTH * 2) + StateManager.MENU_WIDTH;
        int minY = 0;
        int maxY = StateManager.MAP_SCREEN_HEIGHT * 2;

        foreach (var (position, glyph) in sidebar)
        {
            var canvasX = position.X + StateManager.MAP_SCREEN_WIDTH * 2 + StateManager.MENU_MARGIN;
            var canvasY = position.Y;

            if (canvasX < minX || canvasX > maxX ||
                canvasY < minY || canvasY > maxY)
                continue;

            if (IsInCanvas(canvasX, canvasY))
            {
                Vector2I pos = new Vector2I(canvasX, canvasY);

                if (currentSidebar.ContainsKey(pos))
                {
                    currentSidebar.Remove(pos);
                }
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
            if (IsInCanvas(position.X, position.Y))
            {
                Console.SetCursorPosition(position.X, position.Y);
                ConsolePrinter.ClearGlyph();
            }
        }

        previousSidebar = new Dictionary<Vector2I, Glyph>(currentSidebar);
        currentSidebar.Clear();
    }

    public static void RenderImage()
    {
        Dictionary<Vector2I, Glyph> image = PlanetView.GetScreen();

        int minX = 0;
        int maxX = (StateManager.MAP_SCREEN_WIDTH * 2);
        int minY = 0;
        int maxY = StateManager.MAP_SCREEN_HEIGHT * 2;

        if (imageRendered)
            return;

        foreach (var (position, glyph) in image)
        {
            var canvasX = (position.X * 2) - StateManager.MAP_SCREEN_WIDTH / 2;
            var canvasY = position.Y - StateManager.MAP_SCREEN_HEIGHT / 4;

            if (canvasX < minX || canvasX > maxX ||
                canvasY < minY || canvasY > maxY)
                continue;

            if (IsInCanvas(canvasX, canvasY))
            {
                Vector2I pos = new Vector2I(canvasX, canvasY);

                if (currentImage.ContainsKey(pos))
                {
                    currentImage.Remove(pos);
                }
                currentImage.Add(pos, glyph);
                previousImage.Remove(pos);

                Console.SetCursorPosition(canvasX, canvasY);
                ConsolePrinter.PrintGlyph(glyph);
            }
        }
        ClearImage();
    }

    private static void ClearImage()
    {
        foreach (var (position, glyph) in previousImage)
        {
            Console.SetCursorPosition(position.X, position.Y);
            ConsolePrinter.ClearGlyph();
        }

        previousImage = new Dictionary<Vector2I, Glyph>(currentImage);
        currentImage.Clear();
    }

    public static void RenderBattleMap()
    {
        Dictionary<Vector2I, Glyph> map = BattleEngine.GetMap();
        Tuple<Vector2I, Glyph> spaceship = BattleEngine.GetSpaceship();

        Vector2I currentSpaceship = spaceship.Item1;

        Console.SetCursorPosition(currentSpaceship.X * 2, currentSpaceship.Y);
        ConsolePrinter.PrintGlyph(spaceship.Item2);

        var currentMap = new Dictionary<Vector2I, Glyph>(map);
        currentMap.Remove(currentSpaceship);

        foreach (var (position, glyph) in currentMap)
        {
            previousBattleMap.Remove(position);
            Console.SetCursorPosition(position.X * 2, position.Y);
            ConsolePrinter.PrintGlyph(glyph);
        }

        foreach (var (position, _) in previousBattleMap)
        {
            Console.SetCursorPosition(position.X * 2, position.Y);
            ConsolePrinter.ClearGlyph();
        }

        if (!currentSpaceship.Equals(previousSpaceship))
        {
            Console.SetCursorPosition(previousSpaceship.X * 2, previousSpaceship.Y);
            ConsolePrinter.ClearGlyph();
        }

        previousSpaceship = currentSpaceship;
        previousBattleMap = currentMap;

        var state = BattleEngine.GetBattleState();

        int hudX = BattleEngine.MAP_WIDTH * 2 + 2; 
        int line = 0;

        void PrintHudLine(string text)
        {
            Console.SetCursorPosition(hudX, line++);
            Console.Write(text.PadRight(40));
        }

        PrintHudLine("=== Battle Status ===");
        PrintHudLine($"Spaceship HP: {state.SpaceshipHealth}");
        PrintHudLine($"Turret HP:    {state.TurretHealth}");
        PrintHudLine($"Winner:       {state.WinnerName}");
        PrintHudLine($"Duration:     {state.BattleDurationSeconds:F2}s");
    }


    public static bool IsInCanvas(int x, int y)
    {
        return x >= 0 && x < Console.BufferWidth && y >= 0 && y < Console.BufferHeight;
    }
}