using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models.GameModels
{
    public class Entity(int id, string name, char glyph, Color color, Vector2I position)
    {
        public int id { get; set; }
        public string name { get; set; }
        public char glyph { get; set; }
        public Color color { get; set; }
        public Vector2I position { get; set; }
    }
}
