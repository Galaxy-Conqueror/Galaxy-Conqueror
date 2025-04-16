using Galaxy.Conqueror.Client.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models.Menu;

public class MenuItem
{
    public string Name { get; set; }

    public MenuItemOperation OnSelect { get; set; }

    public ConsoleColor Color { get; set; }

    public MenuItem(string name, MenuItemOperation onSelect, ConsoleColor color)
    {
        Name = name;
        OnSelect = onSelect;
        Color = color;
    }
}
