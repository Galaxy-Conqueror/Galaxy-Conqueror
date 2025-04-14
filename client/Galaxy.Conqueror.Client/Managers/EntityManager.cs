using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
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
            Entities.Add(new Spaceship(1, "Spaceship", new Glyph('V', ConsoleColor.Yellow), new Vector2I(StateManager.MAP_WIDTH / 2, StateManager.MAP_HEIGHT / 2)));

            Entities.Add(new Planet(2, "Planet-test", new Glyph('O', ConsoleColor.Blue), new Vector2I(20, 20)));
            Entities.Add(new Planet(3, "Planet-test", new Glyph('O', ConsoleColor.Blue), new Vector2I(25, 25)));
            Entities.Add(new Planet(4, "Planet-test", new Glyph('O', ConsoleColor.Blue), new Vector2I(1, 30)));
            Entities.Add(new Planet(5, "Planet-test", new Glyph('O', ConsoleColor.Blue), new Vector2I(30, 10)));
            Entities.Add(new Planet(6, "Planet-test", new Glyph('O', ConsoleColor.Blue), new Vector2I(7, 0)));
        }
    }
}
