using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.Menu;
using Galaxy.Conqueror.Client.Operations.MenuOperations;
using Galaxy.Conqueror.Client.Utils;
using System.Drawing;

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
        public int ResourceReserve { get; set; }

        public bool Landed { get; set; } = false;

        public Spaceship(int id, string name, Glyph glyph, Vector2I position, string design) : base(id, name, glyph, position)
        {
            Id = id;
            Name = name;
            Glyph = glyph;
            Position = position;
            Design = design;
        }

        public List<MenuItem> GetShipOperations(List<MenuItem> menuItems)
        {
            var isNextToPlanet = EntityManager.Entities.Where(x => x != this).Any(x => Position.DistanceTo(x.Position) <= 1);

            if (Landed)
            {
                menuItems.Add(new MenuItem("Takeoff", TakeoffFromPlanet));
            }
            else if (isNextToPlanet)
            {

                menuItems.Add(new MenuItem("Land on planet", LandOnPlanet));
            }


            return menuItems;
        }

        public void LandOnPlanet()
        {
            StateManager.State = GameState.PLANET_MANAGEMENT;
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
    }
}
