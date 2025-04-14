﻿using Galaxy.Conqueror.Client.Utils;
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
        public Planet(int id, string name, Glyph glyph, Vector2I position) : base(id, name, glyph, position)
        {
            this.Id = id;
            this.Name = name;
            this.Glyph = glyph;
            this.Position = position;
        }
    }
}
