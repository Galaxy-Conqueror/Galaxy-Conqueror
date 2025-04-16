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


        public Planet(int id, Guid userId, string name, Glyph glyph, Vector2I position, string description, int resourceReserve) : base(id, name, glyph, position)
        {
            this.Id = id;
            this.UserId = userId;
            this.Name = name;
            this.Glyph = glyph;
            this.Position = position;
            this.Description = description;
            this.ResourceReserve = resourceReserve;
        }

        public List<MenuItem> GetPlanetOperations(List<MenuItem> menuItems)
        {
            var isOwnPlanet = UserId == AuthHelper.UserId;

            if (isOwnPlanet)
            {
                menuItems.Add(new MenuItem("Upgrade extractor [Cost: 100]", UpgradeResourceExtractor));
                menuItems.Add(new MenuItem("Upgrade turret [Cost: 100]", UpgradeTurret));
            } else
            {
                menuItems.Add(new MenuItem("Attack", AttackPlanet));
            }


                return menuItems;
        }

        public async void UpgradeResourceExtractor()
        {
            var response = await ApiService.UpgradeResourceExtractorAsync();
        }

        public async void UpgradeTurret()
        {
            var response = await ApiService.UpgradeTurretAsync();
        }

        public void TravelToPlanet()
        {
            StateManager.State = GameState.MAP_VIEW;
        }

        public async void AttackPlanet()
        {
            StateManager.State = GameState.BATTLE;

            var response = await BattleService.GetBattleAsync(Id);

            Spaceship ship = (Spaceship)EntityManager.Entities.FirstOrDefault(x => x.Id == StateManager.PlayerShipID);
            if (ship == null) return;

            Spaceship spaceship = new Spaceship(ship.Id, ship.Name, new Glyph('⋀', ConsoleColor.Yellow), new Vector2I(0, 0), response.SpaceshipDesign);
            spaceship.Level = ship.Level;
            spaceship.MaxResources = response.SpaceshipMaxResources;
            spaceship.ResourceReserve = response.SpaceshipResourceReserve;
            spaceship.CurrentHealth = response.SpaceshipHealth;
            spaceship.Damage = response.SpaceshipDamage;

            Turret turret = new Turret(Id, Name, new Glyph('V', ConsoleColor.Red), new Vector2I(0, 0));
            turret.CurrentHealth = response.TurretHealth;
            turret.Damage = response.TurretDamage;
            turret.Level = 1;

            BattleEngine.Initialise(StateManager.MAP_SCREEN_WIDTH, StateManager.MAP_SCREEN_HEIGHT, spaceship, turret, response.PlanetResourceReserve);

            BattleEngine.OnBattleConcluded(async battleResult =>
            { 
                Console.Clear();

                Console.WriteLine($"Started At: {battleResult.StartedAt}");
                Console.WriteLine($"Ended At: {battleResult.EndedAt}");
                Console.WriteLine($"Damage to Spaceship: {battleResult.DamageToSpaceship}");
                Console.WriteLine($"Damage to Turret: {battleResult.DamageToTurret}");
                Console.WriteLine($"Resources Looted: {battleResult.ResourcesLooted}");

                var battleResultResponse = await BattleService.LogBattleAsync(Id, battleResult);

                ship.ResourceReserve += battleResultResponse.ResourcesLooted;

                Thread.Sleep(4000);

                Console.Clear();
                StateManager.State = GameState.PLANET_VIEW;
            });
        }
    }
}
