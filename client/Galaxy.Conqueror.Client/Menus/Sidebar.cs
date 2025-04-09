using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models;


namespace Galaxy.Conqueror.Client.Menus;

public static class Sidebar
{
    public static List<MenuItem> sidebarItems = new List<MenuItem>();
    private static readonly char[,] sidebar = new char[ StateManager.MAP_WIDTH, StateManager.MAP_HEIGHT];
    public static bool stale = true;

    public static char[,] GetSidebar()
    {
        MockMenu();

        stale = true;

        return sidebar;
    }


    public static void MockMenu()
    {
        for (int y = 0; y <  StateManager.MAP_HEIGHT; y++)
        {
            for (int x = 0; x <  StateManager.MAP_WIDTH; x++)
            {
                if (x ==  StateManager.MAP_WIDTH - 1 || x == 0)
                {
                    sidebar[x, y] = '|';
                }
                else if (y ==  StateManager.MAP_HEIGHT - 1 || y == 0)
                {
                    sidebar[x, y] = '-';
                }

            }
            Console.WriteLine();
        }

        WriteMenuLine(5, "a. Testing");
    }

    private static void WriteMenuLine(int index, string line)
    {
        if (line.Length +  StateManager.MENU_MARGIN <  StateManager.MAP_WIDTH)
        {
            for (int i = 0; i < line.Length; i++)
            {
                sidebar[ StateManager.MENU_MARGIN + i, index] = line[i];
            }
        }
    }

}
