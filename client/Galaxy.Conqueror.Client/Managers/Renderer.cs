using System.Collections.Generic;
using System.Text;
using Galaxy.Conqueror.Client.Menus;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
namespace Galaxy.Conqueror.Client.Managers;

public static class Renderer
{
    private static readonly char[,] canvas = new char[StateManager.canvasWidth, StateManager.canvasHeight];
    private static readonly char[,] previousCanvas = new char[StateManager.canvasWidth, StateManager.canvasHeight];

    public static bool stale = true;

    private static readonly StringBuilder outputBuffer = new StringBuilder(StateManager.canvasWidth * StateManager.canvasHeight * 2);

    private static readonly int bufferWidth = Console.BufferWidth;
    private static readonly int bufferHeight = Console.BufferHeight;

    public static void DrawCanvas(char[,] gameScreen, char[,] sidebar)
    {

        if (gameScreen != null)
        {
            AddGameScreenToCanvas(gameScreen);
            MapView.stale = false;
        }

        if (sidebar != null)
        {
            AddSidebarToCanvas(sidebar);
            Sidebar.stale = false;
        }

        if (stale)
        {
            DisableConsoleScrolling();
            RenderChanges();

            Buffer.BlockCopy(canvas, 0, previousCanvas, 0, sizeof(char) * canvas.Length);
            stale = false;
        }

        RenderEntities();
    }

    private static void RenderChanges()
    {
        int lastX = -1, lastY = -1;

        for (int y = 0; y < StateManager.canvasHeight; y++)
        {
            for (int x = 0; x < StateManager.canvasWidth; x++)
            {
                var currentGlyph = canvas[x, y];
                var previousGlyph = previousCanvas[x, y];

                if (currentGlyph != previousGlyph)
                {
                    int adjustedX = x < StateManager.MAP_WIDTH ? x * 2 : x + StateManager.MAP_WIDTH;

                    if (adjustedX < bufferWidth && y < bufferHeight)
                    {
                        if (adjustedX != lastX || y != lastY)
                        {
                            Console.SetCursorPosition(adjustedX, y);
                            lastX = adjustedX;
                            lastY = y;
                        }

                        char displayChar = currentGlyph == 'P' ? '*' : currentGlyph;
                        string color = ConsolePrinter.WHITE;
                        ConsolePrinter.Print(displayChar + " ", color);

                        lastX += 2;
                    }
                }
            }
        }
    }

    private static void RenderEntities()
    {
        int bufferWidth = Console.BufferWidth;
        int bufferHeight = Console.BufferHeight;

        EntityManager.Entities.ForEach(entity =>
        {
            char displayChar = entity.glyph;
            string color = ConsolePrinter.YELLOW;
            Vector2I previousPosition;

            var noPrevPosition = !EntityManager.PrevEntityPositions.TryGetValue(entity.id, out previousPosition);

            if (noPrevPosition) previousPosition = Vector2I.MAX;

            if (previousPosition != entity.position)
            { 
                Console.SetCursorPosition(entity.position.X * 2, entity.position.Y);
                ConsolePrinter.Print(displayChar.ToString(), color);

                RefreshTile(previousPosition);

                EntityManager.PrevEntityPositions[entity.id] = new Vector2I(entity.position);
            }
        });

    }

    private static void RefreshTile(Vector2I position)
    {
        int adjustedX = position.X < StateManager.MAP_WIDTH ? position.X * 2 : position.X + StateManager.MAP_WIDTH;

        if (adjustedX < bufferWidth && position.Y < bufferHeight)
        {

            char displayChar = canvas[position.X, position.Y] == 'P' ? '*' : canvas[position.X, position.Y];
            string color = ConsolePrinter.WHITE;

            Console.SetCursorPosition(position.X * 2, position.Y);
            ConsolePrinter.Print(displayChar + " ", color);
        }
    }

    private static void AddGameScreenToCanvas(char[,] gameScreen)
    {
        for (int y = 0; y < StateManager.MAP_HEIGHT; y++)
        {
            for (int x = 0; x < StateManager.MAP_WIDTH; x++)
            {
                if (canvas[x, y] != gameScreen[x, y])
                {
                    canvas[x, y] = gameScreen[x, y];
                    stale = true;
                }
            }
        }
    }

    private static void AddSidebarToCanvas(char[,] sidebar)
    {
        int baseX = StateManager.MAP_WIDTH + StateManager.MENU_MARGIN;
        for (int y = 0; y < StateManager.MAP_HEIGHT; y++)
        {
            for (int x = 0; x < StateManager.MENU_WIDTH; x++)
            {
                int canvasX = baseX + x;
                if (canvas[canvasX, y] != sidebar[x, y])
                {
                    canvas[canvasX, y] = sidebar[x, y];
                    stale = true;
                }
            }
        }
    }

    private static void DisableConsoleScrolling()
    {
        try
        {
            int requiredWidth = StateManager.canvasWidth + 5;
            int requiredHeight = StateManager.canvasHeight + 2;
            int largestWidth = Console.LargestWindowWidth;
            int largestHeight = Console.LargestWindowHeight;

            int targetWidth = Math.Min(requiredWidth, largestWidth);
            int targetHeight = Math.Min(requiredHeight, largestHeight);

            if (Console.WindowWidth != targetWidth || Console.WindowHeight != targetHeight)
            {
                Console.SetWindowSize(targetWidth, targetHeight);
                Console.SetBufferSize(targetWidth, targetHeight);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not adjust console: {ex.Message}");
            Thread.Sleep(2000);
            Console.Clear();
        }
    }
}