using Galaxy.Conqueror.Client.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models.Menu;

public class MenuItem(string name, MenuItemOperation onSelect)
{
    public string Name { get; set; } = name;

    public MenuItemOperation OnSelect { get; set; } = onSelect;
}
