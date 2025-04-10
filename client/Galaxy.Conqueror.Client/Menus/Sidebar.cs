using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models;
using Galaxy.Conqueror.Client.Utils;


namespace Galaxy.Conqueror.Client.Menus;

public static class Sidebar
{
    public static List<MenuItem> sidebarItems = new List<MenuItem>();
    private static readonly Dictionary<Vector2I, char> sidebar = new();
    public static bool stale = true;

    public static Dictionary<Vector2I, char> GetSidebar()
    {
        MockMenu();

        stale = false;

        return sidebar;
    }

    public static void MockMenu()
    {
        var maxY = (StateManager.MAP_SCREEN_HEIGHT / 2) - StateManager.MENU_MARGIN;

        for (int y = 0; y <  maxY; y++)
        {
            for (int x = 0; x <  StateManager.MENU_WIDTH; x++)
            {
                if (x ==  StateManager.MENU_WIDTH - 1 || x == 0)
                {
                    sidebar[new Vector2I(x, y)] = '|';
                }
                else if (y == maxY - 1 || y == 0)
                {
                    sidebar[new Vector2I(x, y)] = '-';
                }

            }
            Console.WriteLine();
        }

        WriteMenuLine(5, "a. Testing");
    }

    private static void WriteMenuLine(int index, string line)
    {
        if (line.Length +  StateManager.MENU_MARGIN <  StateManager.MENU_WIDTH)
        {
            for (int i = 0; i < line.Length; i++)
            {
                var position = new Vector2I(StateManager.MENU_MARGIN + i, index);

                if (sidebar.ContainsKey(position)) 
                {
                    sidebar[position] = line[i];
                }
                else
                {
                    sidebar.Add(position, line[i]);
                }
                    
            }
        }
    }

}
