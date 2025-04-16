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
        public int UpgradeCost { get; set; }
        public int Damage { get; set; }
        public int MaxFuel { get; set; }
        public int MaxResources { get; set; }

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
            CurrentHealth = Math.Max(CurrentHealth, 0);
        }

        public bool IsDestroyed()
        {
            return CurrentHealth <= 0;
        }

        public Spaceship ConvertFromRemoteSpaceship()
        {

            Design = Design.Replace("\n", "\r\n").Replace("\\n", "\r\n");
            Id = Id + 999;

            Glyph = new Glyph('^', ConsoleColor.White);
            Position = new Vector2I(X, Y);

            return this;
        }

        public async Task<List<MenuItem>> GetShipOperations(List<MenuItem> menuItems)
        {
            var adjacentEntity = EntityManager.Entities.Where(x => x != this).FirstOrDefault(x => Position.DistanceTo(x.Position) <= 2);

            if (adjacentEntity is Planet adjacentPlanet)
            {

                if (Landed)
                {
                    menuItems.Add(new MenuItem("Leave orbit", TakeoffFromPlanet, ConsoleColor.White));

                    var isOwnPlanet = adjacentPlanet.UserId == AuthHelper.UserId;

                    if (isOwnPlanet)
                    {
                        int repairCost = MaxHealth - CurrentHealth;

                        if (repairCost <= StateManager.PlayerPlanet.ResourceReserve)
                        {
                            menuItems.Add(new MenuItem($"Repair [Cost: {repairCost}]", Repair, ConsoleColor.White));
                        }
                        else
                        {
                            menuItems.Add(new MenuItem($"Repair [Cost: {repairCost}]", Repair, ConsoleColor.Red));
                        }

                        if (UpgradeCost <= StateManager.PlayerPlanet.ResourceReserve)
                        {
                            menuItems.Add(new MenuItem($"Upgrade ship [Cost: {UpgradeCost}]", Upgrade, ConsoleColor.White));
                        }
                        else
                        {
                            menuItems.Add(new MenuItem($"Upgrade ship [Cost: {UpgradeCost}]", () => { }, ConsoleColor.Red));
                        }

                        if (ResourceReserve > 0)
                        {
                            menuItems.Add(new MenuItem($"Deposit [Material: {ResourceReserve}]", Deposit, ConsoleColor.White));
                        }
                        else
                        {
                            menuItems.Add(new MenuItem($"Deposit [Material: {ResourceReserve}]", () => { }, ConsoleColor.Red));
                        }

                    }

                    if (isOwnPlanet)
                    {
                        await StateManager.PlayerPlanet.GetPlanetOperations(menuItems);
                    }
                    else
                    {
                        await adjacentPlanet.GetPlanetOperations(menuItems);
                    }

                }

                if (!Landed && adjacentPlanet != null)
                {
                    menuItems.Add(new MenuItem($"Enter orbit around {adjacentPlanet.Name}", LandOnPlanet, ConsoleColor.White));
                }


            }

            if (!Landed)
            {
                menuItems.Add(new MenuItem($"Warp back to home {StateManager.PlayerPlanet.Name}", WarpHome, ConsoleColor.White));
            }


            return menuItems;
        }

        public void WarpHome()
        {
            StateManager.PlayerSpaceship.Position = StateManager.PlayerPlanet.Position;
        }

        public async Task UpdateShipState()
        {
            var serverState = await ApiService.GetSpaceshipAsync();

            UserId = serverState.UserId;
            Level = serverState.Level;
            CurrentFuel = serverState.CurrentFuel;
            CurrentHealth = serverState.CurrentHealth;
            MaxHealth = serverState.MaxHealth;
            ResourceReserve = serverState.ResourceReserve;
            UpgradeCost = serverState.UpgradeCost;
            Damage = serverState.Damage;
            MaxFuel = serverState.MaxFuel;
            MaxResources = serverState.MaxResources;

            if (!string.IsNullOrEmpty(serverState.Name))
            {
                Name = serverState.Name;
            }

            StateManager.PlayerSpaceship = this;
        }

        public async void LandOnPlanet()
        {
            StateManager.State = GameState.PLANET_VIEW;

            //Update ship state 
            await StateManager.UpdateOwnPlanet();
            await UpdateShipState();
            StateManager.CurrentExtractor = await ApiService.GetOwnExtractor();
            StateManager.CurrentTurret = await ApiService.GetOwnTurret();

            Landed = true;
        }

        public async void TakeoffFromPlanet()
        {
            StateManager.State = GameState.MAP_VIEW;
            await StateManager.UpdateOwnPlanet();
            await UpdateShipState();
            Landed = false;
        }

        public void TravelToPlanet()
        {
            StateManager.State = GameState.MAP_VIEW;
        }

        public async void Refuel()
        {
            var response = ApiService.RefuelSpaceshipAsync();
            await UpdateShipState();
            await StateManager.UpdateOwnPlanet();
        }

        public async void Repair()
        {
            await ApiService.RepairSpaceshipAsync();
            await UpdateShipState();
            await StateManager.UpdateOwnPlanet();
        }

        public async void Upgrade()
        {
            var response = await ApiService.UpgradeSpaceshipAsync();

            MaxHealth = response.MaxHealth;
            Damage = response.Damage;
            MaxFuel = response.MaxFuel;
            MaxResources = response.MaxResources;

            await UpdateShipState();
            await StateManager.UpdateOwnPlanet();
        }

        public async void Deposit()
        {
            var response = ApiService.DepositAsync();
            await UpdateShipState();
            await StateManager.UpdateOwnPlanet();
        }

    }
}
