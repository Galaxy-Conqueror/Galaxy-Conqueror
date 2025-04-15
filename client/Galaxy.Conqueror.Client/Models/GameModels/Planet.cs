using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models.GameModels
{
    public class Planet : Entity
    {
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Design { get; set; }
        public string? Description { get; set; }
        public int ResourceReserve { get; set; }

        public int X { get; set; }

        public int Y { get; set; }


        public Planet(int id, string name, Glyph glyph, Vector2I position, string description, int resourceReserve) : base(id, name, glyph, position)
        {
            this.Id = id;
            this.Name = name;
            this.Glyph = glyph;
            this.Position = position;
            this.Description = description;
            this.ResourceReserve = resourceReserve;
        }
    }
}
