using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models
{
    interface IView
    {

        public static abstract void Initialise();

        public static abstract Dictionary<Vector2I, char> GetScreen();
    }
}
