using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Managers
{
    public static class EntityManager
    {
        public static List<Entity> Entities { get; set; } = [];

        public static Dictionary<int, Vector2I> PrevEntityPositions { get; set; } = [];

        public static async Task Initialize()
        {
            //Entities.Add(new Spaceship(1, "Spaceship", new Glyph('V', ConsoleColor.Yellow), new Vector2I(StateManager.MAP_WIDTH / 2, StateManager.MAP_HEIGHT / 2), "SSSS△SSSS\r\nSS:▓╬▓:SS\r\nS:╔▒╬▒╗:S\r\n:▓▓◙█◙▓▓:\r\n█╗▓▓╬▓▓╔█\r\nSS▼▼▼▼▼▼SS"));
            //Entities.Add(new Spaceship(1, "Spaceship", new Glyph('V', ConsoleColor.Yellow), new Vector2I(StateManager.MAP_WIDTH / 2, StateManager.MAP_HEIGHT / 2), "SSSS║SSSS\r\nSS◣▓╦▓◢SS\r\nS◤╠▒▀▒╣◥S\r\n╔▲◘╬█╬◘▲╗\r\n◄▒╝▓┼▓╚▒►\r\nSS▼▼▼▼▼▼SS"));

            Entities.Add(new Planet(2, "Planet-test", new Glyph('O', ConsoleColor.Blue), new Vector2I(StateManager.MAP_WIDTH / 2, StateManager.MAP_HEIGHT / 2), "Test", 0));
           
            var planets = await ApiService.GetPlanetsAsync();

            foreach (var planet in planets)
            {
                Entities.Add(new Planet(planet.Id, planet.Name, new Glyph('O', ConsoleColor.Blue), new Vector2I(planet.X, planet.Y), planet.Description, planet.ResourceReserve));
            }

            var playerShip = (await ApiService.GetSpaceshipAsync()).ConvertFromRemoteSpaceship();

            Entities.Add(playerShip);
            StateManager.PlayerShipID = playerShip.Id;
        }
    }
}
