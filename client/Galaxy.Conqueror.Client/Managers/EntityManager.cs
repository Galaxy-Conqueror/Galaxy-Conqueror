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
            var planets = await ApiService.GetPlanetsAsync();

            foreach (var planet in planets)
            {
                var newPlanet = new Planet(planet.Id, planet.UserId, planet.Name, new Glyph('O', ConsoleColor.Blue), new Vector2I(planet.X, planet.Y), planet.Description, planet.Design, planet.ResourceReserve);
                Entities.Add(newPlanet);
            }

            await StateManager.UpdateOwnPlanet();

            StateManager.PlayerSpaceship = (await ApiService.GetSpaceshipAsync()).ConvertFromRemoteSpaceship();

            Entities.Add(StateManager.PlayerSpaceship);
        }
    }
}
