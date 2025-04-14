using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Models.Menu;
using Galaxy.Conqueror.Client.Operations.MenuOperations;
using Galaxy.Conqueror.Client.Utils;


namespace Galaxy.Conqueror.Client.Menus;

public static class Sidebar
{
    public static Menu Content;
    private static readonly Dictionary<Vector2I, Glyph> sidebar = new();
    public static bool stale = true;

    public static Dictionary<Vector2I, Glyph> GetSidebar()
    {
        MockMenu();

        stale = false;

        return sidebar;
    }

    public static void MockMenu()
    {
        Content = new Menu(new List<MenuItem>());

        UpdateSidebarState();

        var maxY = (StateManager.MAP_SCREEN_HEIGHT / 2) - StateManager.MENU_MARGIN;

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
            Console.WriteLine();
        }

        var itemStrings = Content.ItemStrings();

        for (int i = 0; i < itemStrings.Count; i++)
        {
            WriteMenuLine(i + 5, itemStrings[i]);
        }
    }

    public static void UpdateSidebarState()
    {
        var playerEntity = EntityManager.Entities.First(x => x.Id == StateManager.PlayerShipID);

        var menuItems = new List<MenuItem>();
        var prevContent = new List<MenuItem>();

        foreach (var x in Content.Items)
        {
            prevContent.Add(x);
        }

        if (playerEntity is Spaceship playerShip)
        {
            playerShip.GetShipOperations(menuItems);
        }

        menuItems.Add(new MenuItem("Pause", GameOperations.Pause));

        menuItems.Add(new MenuItem("Resume", GameOperations.Resume));

        menuItems.Add(new MenuItem("Quit", GameOperations.Quit));

        Content.Items = menuItems.ToArray();

        if (MenuChanged(prevContent, Content.Items.ToList()))
        {
            stale = true;
        }
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

        return false;
    }

    private static void WriteMenuLine(int index, string line)
    {
        if (line.Length + StateManager.MENU_MARGIN < StateManager.MENU_WIDTH)
        {
            for (int i = 0; i < StateManager.MENU_WIDTH - StateManager.MENU_MARGIN; i++)
            {
                var position = new Vector2I(StateManager.MENU_MARGIN + i, index);

                if (sidebar.ContainsKey(position) && i < line.Length)
                {
                    sidebar[position] = new Glyph(line[i], ConsoleColor.White);
                }
                else if (i < line.Length)
                {
                    sidebar.Add(position, new Glyph(line[i], ConsoleColor.White));
                }
                else
                {
                    sidebar.Remove(position);
                }
            }
        }
    }

}
