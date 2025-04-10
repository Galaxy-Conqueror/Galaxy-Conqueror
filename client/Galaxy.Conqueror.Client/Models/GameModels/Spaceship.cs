﻿using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models.GameModels
{
    public class Spaceship : Entity
    {
        public Spaceship(int id, string name, char glyph, Color color, Vector2I position) : base(id, name, glyph, color, position)
        {
            this.Id = id;
            this.Name = name;
            this.Glyph = glyph;
            this.Color = color;
            this.Position = position;
        }
    }
}
