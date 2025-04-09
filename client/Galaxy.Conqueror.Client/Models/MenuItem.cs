using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models;

public class MenuItem
{
    private string name { get; set; }

    public MenuItem(string name)
    {
        this.name = name;
    }
}
