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
        public int Id { get; set; }
        public string Name { get; set; }
        public char Glyph { get; set; }
        public Color Color { get; set; }
        public Vector2I Position { get; set; }
        public bool Stale { get; set; } = true;
    }
}
