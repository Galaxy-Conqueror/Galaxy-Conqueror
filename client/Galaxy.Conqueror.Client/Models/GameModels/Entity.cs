using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models.GameModels
{
    public class Entity(int id, string name, Glyph glyph, Vector2I position)
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Glyph Glyph { get; set; }
        public Vector2I Position { get; set; }
    }
}
