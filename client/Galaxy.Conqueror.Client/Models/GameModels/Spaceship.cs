using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.Menu;
using Galaxy.Conqueror.Client.Operations.MenuOperations;
using Galaxy.Conqueror.Client.Utils;
using System.Drawing;
using System.Xml.Linq;

namespace Galaxy.Conqueror.Client.Models.GameModels
{
    public class Spaceship : Entity
    {
        public Guid UserId { get; set; }
        public string? Design { get; set; }
        public string? Description { get; set; }
        public int Level { get; set; }
        public int CurrentFuel { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int ResourceReserve { get; set; }
        public int MaxResources { get; set; }
        public int Damage { get; set; }

        public bool Landed { get; set; } = false;
        public int X { get; set; }
        public int Y { get; set; }

        public Spaceship(int id, string name, Glyph glyph, Vector2I position, string design) : base(id, name, glyph, position)
        {
            Id = id;
            Name = name;
            Glyph = glyph;
            Position = position;
            Design = design;
        }

        public void TakeDamage(Bullet bullet)
        {
            CurrentHealth -= bullet.Damage;
        }

        public bool IsDestroyed()
        {
            return CurrentHealth <= 0;
        }

        public Spaceship ConvertFromRemoteSpaceship()
        {
            Design = Design.Replace("\\n", "\r\n");
            Id += 999;
            Glyph = new Glyph('^', ConsoleColor.White);
            Position = new Vector2I(X, Y);

            return this;
        }

        public List<MenuItem> GetShipOperations(List<MenuItem> menuItems)
        {
            var adjacentEntity = EntityManager.Entities.Where(x => x != this).FirstOrDefault(x => Position.DistanceTo(x.Position) <= 1);

            if (adjacentEntity is Planet adjacentPlanet)
            {
                if (Landed)
                {
                    menuItems.Add(new MenuItem("Takeoff", TakeoffFromPlanet));

                    menuItems.Add(new MenuItem("Refuel [Cost: 100]", Refuel));
                    menuItems.Add(new MenuItem("Repair [Cost: 100]", Repair));
                    menuItems.Add(new MenuItem("Upgrade [Cost: 100]", Upgrade));
                    menuItems.Add(new MenuItem("Deposit [Material: 100]", Deposit));

                    adjacentPlanet.GetPlanetOperations(menuItems);          
                }
                else if (adjacentPlanet != null)
                {
                    menuItems.Add(new MenuItem($"Enter orbit around {adjacentPlanet.Name}", LandOnPlanet));
                }
            }


            return menuItems;
        }

        public void LandOnPlanet()
        {
            StateManager.State = GameState.PLANET_VIEW;
            Landed = true;
        }

        public void TakeoffFromPlanet()
        {
            StateManager.State = GameState.MAP_VIEW;
            Landed = false;
        }

        public void TravelToPlanet()
        {
            StateManager.State = GameState.MAP_VIEW;
        }

        public async void Refuel()
        {
            var reponse = ApiService.RefuelSpaceshipAsync();
        }

        public async void Repair()
        {
            var reponse = ApiService.RepairSpaceshipAsync();
        }

        public async void Upgrade()
        {
            var reponse = ApiService.UpgradeSpaceshipAsync();
        }

        public async void Deposit()
        {
            var reponse = ApiService.DepositAsync();
        }

    }
}
