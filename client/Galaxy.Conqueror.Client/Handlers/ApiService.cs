using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Handlers
{
    public static class ApiService
    {
        public static async Task<IEnumerable<Planet>> GetPlanetsAsync()
        {
            try
            {
                var planetResponse = await RequestHelper.GetRequestAsync("/api/planets");

                var content = await planetResponse.Content.ReadFromJsonAsync<IEnumerable<Planet>>();

                return content ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the get operation: {ex.Message}", ex);
            }
        }

        public static async Task<Spaceship> GetSpaceshipAsync()
        {
            try
            {
                var spaceshipResponse = await RequestHelper.GetRequestAsync("/api/spaceship");

                var spaceship = await spaceshipResponse.Content.ReadFromJsonAsync<Spaceship>();

                return spaceship; //?? new Spaceship(spaceship.Id, spaceship.Name, new Glyph('V', ConsoleColor.Yellow), new Vector2I(spaceship.X, spaceship.Y), "SSSS║SSSS\r\nSS◣▓╦▓◢SS\r\nS◤╠▒▀▒╣◥S\r\n╔▲◘╬█╬◘▲╗\r\n◄▒╝▓┼▓╚▒►\r\nSS▼▼▼▼▼▼SS");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the get spaceship operation: {ex.Message}", ex);
            }
        }
    }
}
