using Battle;
using Galaxy.Conqueror.API.Models.Responses;
using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.Menu;
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


        public Planet(int id, Guid userId, string name, Glyph glyph, Vector2I position, string description, string design, int resourceReserve) : base(id, name, glyph, position)
        {
            this.Id = id;
            this.UserId = userId;
            this.Name = name;
            this.Glyph = glyph;
            this.Position = position;
            this.Description = description;
            Design = design;
            this.ResourceReserve = resourceReserve;
        }

        public async Task<List<MenuItem>> GetPlanetOperations(List<MenuItem> menuItems)
        {
            var isOwnPlanet = UserId == AuthHelper.UserId;

            if (isOwnPlanet)
            {
                if (ResourceReserve > StateManager.CurrentExtractor.UpgradeCost)
                {
                    menuItems.Add(new MenuItem($"Upgrade extractor [Cost: {StateManager.CurrentExtractor.UpgradeCost}]", UpgradeResourceExtractor, ConsoleColor.White));
                }
                else
                {
                    menuItems.Add(new MenuItem($"Upgrade extractor [Cost: {StateManager.CurrentExtractor.UpgradeCost}]", () => { }, ConsoleColor.Red));
                }

                if (ResourceReserve > StateManager.CurrentTurret.UpgradeCost)
                {
                    menuItems.Add(new MenuItem($"Upgrade turret [Cost: {StateManager.CurrentTurret.UpgradeCost}]", UpgradeTurret, ConsoleColor.White));
                }
                else
                {
                    menuItems.Add(new MenuItem($"Upgrade turret [Cost: {StateManager.CurrentTurret.UpgradeCost}]", () => { }, ConsoleColor.Red));
                }
            }
            else
            {
                menuItems.Add(new MenuItem("Attack", AttackPlanet, ConsoleColor.White));
            }
            menuItems.Add(new MenuItem("Show planet description", () =>
            {
                StateManager.State = GameState.INTRO_VIEW;
            }, ConsoleColor.White));

            return menuItems;
        }

        public async void UpgradeResourceExtractor()
        {
            await ApiService.UpgradeResourceExtractorAsync();
            await StateManager.UpdateExtractor();

            await StateManager.UpdateOwnPlanet();

            StateManager.CurrentExtractor = await ApiService.GetOwnExtractor();
        }

        public async void UpgradeTurret()
        {
            ApiService.UpgradeTurretAsync();
            StateManager.PlayerSpaceship.UpdateShipState();
            await StateManager.UpdateOwnPlanet();
            StateManager.CurrentTurret = await ApiService.GetOwnTurret();
        }

        public void TravelToPlanet()
        {
            StateManager.State = GameState.MAP_VIEW;
        }

        public async void AttackPlanet()
        {
            StateManager.State = GameState.BATTLE;

            var response = await BattleService.GetBattleAsync(Id);
            if (response == null || StateManager.PlayerSpaceship == null) return;

            var ship = StateManager.PlayerSpaceship;

            var spaceship = new Spaceship(ship.Id, ship.Name, new Glyph('⋀', ConsoleColor.Yellow), new Vector2I(0, 0), response.SpaceshipDesign)
            {
                Level = response.SpaceshipLevel,
                MaxResources = response.SpaceshipMaxResources,
                ResourceReserve = response.SpaceshipResourceReserve,
                CurrentHealth = response.SpaceshipHealth,
                Damage = response.SpaceshipDamage
            };

            var turret = new Turret(Id, Name, new Glyph('V', ConsoleColor.Red), new Vector2I(0, 0))
            {
                CurrentHealth = response.TurretHealth,
                Level = response.TurretLevel,
                Damage = response.TurretDamage
            };

            BattleEngine.Initialise(spaceship, turret, response.PlanetResourceReserve);

            BattleEngine.OnBattleConcluded(async result =>
            {
                if (result == null) return;

                var logResponse = await BattleService.LogBattleAsync(Id, result);
                if (logResponse != null)
                    ship.ResourceReserve += logResponse.ResourcesLooted;

                Console.Clear();
                StateManager.State = GameState.PLANET_VIEW;
            });
        }

    }
}
