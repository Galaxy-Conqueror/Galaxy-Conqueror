using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Models;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Managers
{
    public static class StateManager
    {
        public static readonly int MENU_WIDTH = int.Parse(ConfigurationManager.AppSettings.Get("MENU_WIDTH") ?? "0");
        public static readonly int MAP_WIDTH = int.Parse(ConfigurationManager.AppSettings.Get("MAP_WIDTH") ?? "0");
        public static readonly int MAP_HEIGHT = int.Parse(ConfigurationManager.AppSettings.Get("MAP_HEIGHT") ?? "0");

        public static readonly int MAP_SCREEN_WIDTH = int.Parse(ConfigurationManager.AppSettings.Get("MAP_SCREEN_WIDTH") ?? "0");
        public static readonly int MAP_SCREEN_HEIGHT = int.Parse(ConfigurationManager.AppSettings.Get("MAP_SCREEN_HEIGHT") ?? "0");

        public static readonly int MENU_MARGIN = int.Parse(ConfigurationManager.AppSettings.Get("MENU_MARGIN") ?? "0");

        public static Spaceship PlayerSpaceship { get; set; } = new(-1, "", new Glyph('V', ConsoleColor.White), new Vector2I(0, 0), "");
        public static Planet PlayerPlanet { get; set; } = new(-1, Guid.NewGuid(), "blank", new Glyph('O', ConsoleColor.Blue), Vector2I.ZERO, "", "", 0);
        public static Turret PlayerTurret { get; set; } = new(0, "blank", new Glyph('T', ConsoleColor.Red), new Vector2I(0,0));
        public static ResourceExtractor PlayerExtractor { get; set; } = new();

        public static int CanvasWidth = (MAP_WIDTH * 2) + MENU_WIDTH + MENU_MARGIN;
        public static int CanvasHeight = MAP_HEIGHT;
        public static GameState State = GameState.MAP_VIEW;

        public async static Task<Planet> UpdatePlanetStateById(Planet planet)
        {
            var serverPlanet = await ApiService.GetPlanetByIdAsync(planet.Id);

            planet.Id = serverPlanet.Id;
            planet.Name = serverPlanet.Name;
            planet.Position = serverPlanet.Position;
            planet.Glyph = serverPlanet.Glyph;
            serverPlanet.ResourceReserve = serverPlanet.ResourceReserve;

            planet.X = serverPlanet.Position.X;
            planet.Y = serverPlanet.Position.Y;

            return planet;
        }

        public static Planet UpdatePlanet(Planet planet, Planet serverPlanet)
        {
            planet.Id = serverPlanet.Id;
            planet.Name = serverPlanet.Name;
            planet.Position = serverPlanet.Position;
            planet.Glyph = serverPlanet.Glyph;
            serverPlanet.ResourceReserve = serverPlanet.ResourceReserve;

            planet.X = serverPlanet.Position.X;
            planet.Y = serverPlanet.Position.Y;

            return planet;
        }

        public async static void UpdateOwnPlanet()
        {
            var serverPlanet = await ApiService.GetPlanetAsync();
            PlayerPlanet.Name = serverPlanet.Name;
            PlayerPlanet.ResourceReserve = serverPlanet.ResourceReserve;
            PlayerPlanet.Position = new Vector2I(serverPlanet.X, serverPlanet.Y);
        }

        public async static Task<ResourceExtractor> UpdateExtractor()
        {
            var serverExtractor = await ApiService.GetOwnExtractor();

            PlayerExtractor = new ResourceExtractor()
            {
                Id = serverExtractor.Id,
                Level = serverExtractor.Level,
                PlanetId = serverExtractor.PlanetId,
                ResourceGen = serverExtractor.ResourceGen,
                UpgradeCost = serverExtractor.UpgradeCost,
                UpgradedResourceGen = serverExtractor.UpgradedResourceGen
            };

            return PlayerExtractor;
        }

        public async static Task<Turret> UpdateTurret()
        {
            var serverTurret = await ApiService.GetOwnTurret();

            PlayerTurret = Turret.GetTurretFromServerModel(serverTurret);

            return PlayerTurret;
        }
    }
}
