using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models.Menu;

public class Menu
{
    public MenuItem[] Items { get; set; }

    public Menu(List<MenuItem> items)
    {
        Items = items.ToArray();
    }

    public List<(string, ConsoleColor)> ItemStrings()
    {
        var stringItems = new List<(string, ConsoleColor)>();

        for (int i = 0; i < Items.Length; i++)
        {
            stringItems.Add(($"{(char)('a' + i)}. {Items[i].Name}", Items[i].Color));
        }

        return stringItems;
    }
}
