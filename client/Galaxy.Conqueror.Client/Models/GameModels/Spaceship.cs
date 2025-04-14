using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.Menu;
using Galaxy.Conqueror.Client.Operations.MenuOperations;
using Galaxy.Conqueror.Client.Utils;
using System.Drawing;

namespace Galaxy.Conqueror.Client.Models.GameModels
{
    public class Spaceship : Entity
    {
        public bool landed { get; set; } = false;

        public Spaceship(int id, string name, Glyph glyph, Vector2I position) : base(id, name, glyph, position)
        {
            Id = id;
            Name = name;
            Glyph = glyph;
            Position = position;
        }

        public List<MenuItem> GetShipOperations(List<MenuItem> menuItems)
        {
            var isNextToPlanet = EntityManager.Entities.Where(x => x != this).Any(x => Position.DistanceTo(x.Position) <= 1);

            if (landed)
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
            landed = true;
        }

        public void TakeoffFromPlanet()
        {
            StateManager.State = GameState.MAP_VIEW;
            landed = false;
        }

        public void TravelToPlanet()
        {
            StateManager.State = GameState.MAP_VIEW;
        }
    }
}
