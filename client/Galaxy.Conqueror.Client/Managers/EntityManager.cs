using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Managers
{
    public static class EntityManager
    {
        public static List<Entity> Entities { get; set; } = [];

        public static Dictionary<int, Vector2I> PrevEntityPositions { get; set; } = [];

        public static void Initialize()
        {
            Entities.Add(new Spaceship(1, "Spaceship", 'V', Color.Yellow, new Vector2I(0, 0)));
        }
    }
}
