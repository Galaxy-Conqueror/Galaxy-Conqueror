using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models.GameModels
{
    public class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Glyph Glyph { get; set; }
        public Vector2I Position { get; set; }

        public Entity()
        {
            Id = int.MaxValue;
            Name = "";
            Glyph = new Glyph(' ', ConsoleColor.Red);
            Position = Vector2I.ZERO;
        }

        public Entity(int id, string name, Glyph glyph, Vector2I position)
        {
            Id = id;
            Name = name;
            Glyph = glyph;
            Position = position;
        }
    }
}
