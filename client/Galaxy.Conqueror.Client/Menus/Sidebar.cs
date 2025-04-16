using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Models.Menu;
using Galaxy.Conqueror.Client.Operations.MenuOperations;
using Galaxy.Conqueror.Client.Utils;
using System.Text;


namespace Galaxy.Conqueror.Client.Menus;

public static class Sidebar
{
    public static Menu Content;
    private static readonly Dictionary<Vector2I, Glyph> sidebar = new();
    public static bool Stale { get; set; } = true;

    public static Dictionary<Vector2I, Glyph> GetSidebar()
    {
        return sidebar;
    }

    public async static void MockMenu()
    {
        Content = new Menu(new List<MenuItem>());

        await CheckSidebarState();

        var maxY = (StateManager.MAP_SCREEN_HEIGHT) - StateManager.MENU_MARGIN;

        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < StateManager.MENU_WIDTH; x++)
            {
                if (x == StateManager.MENU_WIDTH - 1 || x == 0)
                {
                    sidebar[new Vector2I(x, y)] = new Glyph('|', ConsoleColor.White);
                }
                else if (y == maxY - 1 || y == 0)
                {
                    sidebar[new Vector2I(x, y)] = new Glyph('-', ConsoleColor.White);
                }

            }
        }

        var itemStrings = Content.ItemStrings();

        for (int i = 0; i < itemStrings.Count; i++)
        {
            WriteMenuLine(i + 5, itemStrings[i]);
        }
    }

    public async static Task CheckSidebarState()
    {
        var menuItems = new List<MenuItem>();
        var prevContent = new List<MenuItem>();

        if (StateManager.State != GameState.INTRO_VIEW)
        {

            foreach (var x in Content.Items)
            {
                prevContent.Add(x);
            }

            await StateManager.PlayerSpaceship.GetShipOperations(menuItems);

            //menuItems.Add(new MenuItem("Pause", GameOperations.Pause));

            menuItems.Add(new MenuItem("Ping home planet (update info)", GameOperations.PingHome, ConsoleColor.Green));

            menuItems.Add(new MenuItem("Quit", GameOperations.Quit, ConsoleColor.White));

            if (MenuChanged(prevContent, Content.Items.ToList()))
            {
                Stale = true;
            }
        } else
        {
            var playerPlanet = EntityManager.Entities.Where((x =>
            {
                if (x is Planet planet)
                {
                    return planet.UserId == AuthHelper.UserId;
                }
                return false;
            })).First() as Planet;

            if (playerPlanet != null)
            {
                var description = playerPlanet?.Description ?? "A planet.";

                int linesUsed = WriteMenuTextWithWordWrap(StateManager.MENU_MARGIN, description);

                WriteMenuTextWithWordWrap(linesUsed + 5, "Press any key to continue...");

                Stale = true;
            }
        }
        Content.Items = menuItems.Where(x => x.Name != "").ToArray();
    }

    private static bool MenuChanged(List<MenuItem> prevItems, List<MenuItem> currItems)
    {
        if (prevItems.Count != currItems.Count)
            return true;

        for (int i = 0; i < prevItems.Count; i++)
        {
            if (prevItems[i].Name != currItems[i].Name)
                return true;
        }

        var itemStrings = Content.ItemStrings();

        ClearMenu();

        for (int i = 0; i < itemStrings.Count; i++)
        {
            WriteMenuLine(i + 2, itemStrings[i]);
        }

        return false;
    }

    private static void ClearMenu()
    {
        var maxY = (StateManager.MAP_SCREEN_HEIGHT) - StateManager.MENU_MARGIN;

        for (int y = 1; y < maxY - 1; y++)
        {
            for (int x = 1; x < StateManager.MENU_WIDTH - 1; x++)
            {
                sidebar.Remove(new Vector2I(x, y));
            }
        }

        WriteMenuTextWithWordWrap(StateManager.MAP_SCREEN_HEIGHT - 17, $"Ship Resource Reserve: {StateManager.PlayerSpaceship.ResourceReserve}");
        WriteMenuTextWithWordWrap(StateManager.MAP_SCREEN_HEIGHT - 16, $"Last Recorded Planet Reserve: {StateManager.PlayerPlanet.ResourceReserve}");

        if (!StateManager.PlayerSpaceship.Landed)
        {
            int count = StateManager.MAP_SCREEN_HEIGHT - 25;

            foreach (string line in StateManager.PlayerSpaceship.Design.Split("\r\n").ToList())
            {
                string spacedLine = string.Join(" ", line.Replace('S', ' ').ToCharArray());
                WriteMenuLine(count, ("          " + spacedLine, ConsoleColor.White));
                count++;
            }
        }
        else
        {
            var adjacentEntity = EntityManager.Entities.Where(x => x != StateManager.PlayerSpaceship).FirstOrDefault(x => StateManager.PlayerSpaceship.Position.DistanceTo(x.Position) <= 2);
            Stale = false;

            if (adjacentEntity is Planet adjacentPlanet)
            {
                WriteMenuTextWithWordWrap(StateManager.MAP_SCREEN_HEIGHT - 14, $"Turret level: {StateManager.CurrentTurret.Level}");
                WriteMenuTextWithWordWrap(StateManager.MAP_SCREEN_HEIGHT - 13, $"Extractor level: {StateManager.CurrentExtractor.Level}");
                WriteMenuTextWithWordWrap(StateManager.MAP_SCREEN_HEIGHT - 12, $"Extraction rate: {StateManager.CurrentExtractor.ResourceGen}");
            }
        }
    }

    private static void WriteMenuLine(int index, (string, ConsoleColor) item)
    {
        var line = item.Item1;
        var color = item.Item2;

        if (line.Length + StateManager.MENU_MARGIN < StateManager.MENU_WIDTH)
        {
            for (int i = 0; i < StateManager.MENU_WIDTH; i++)
            {
                var position = new Vector2I(StateManager.MENU_MARGIN + i, index);

                if (sidebar.ContainsKey(position) && i < line.Length)
                {
                    sidebar[position] = new Glyph(line[i], color);
                }
                else if (i < line.Length)
                {
                    sidebar.Add(position, new Glyph(line[i], color));
                }
                else
                {
                    sidebar.Remove(position);
                }

            }
        }
    }

    public static int WriteMenuTextWithWordWrap(int startIndex, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        int currentLineIndex = startIndex;
        int lineWidth = StateManager.MENU_WIDTH - (StateManager.MENU_MARGIN * 2);
        List<string> words = text.Split(' ').ToList();

        StringBuilder currentLine = new StringBuilder();

        foreach (string word in words)
        {
            if (currentLine.Length + word.Length + (currentLine.Length > 0 ? 1 : 0) > lineWidth)
            {
                WriteMenuLine(currentLineIndex, (currentLine.ToString(), ConsoleColor.White));
                currentLineIndex++;

                currentLine.Clear();
                currentLine.Append(word);
            }
            else
            {
                if (currentLine.Length > 0)
                {
                    currentLine.Append(' ');
                }
                currentLine.Append(word);
            }
        }

        if (currentLine.Length > 0)
        {
            WriteMenuLine(currentLineIndex, (currentLine.ToString(), ConsoleColor.White));
            currentLineIndex++;
        }

        return currentLineIndex - startIndex;
    }
}
