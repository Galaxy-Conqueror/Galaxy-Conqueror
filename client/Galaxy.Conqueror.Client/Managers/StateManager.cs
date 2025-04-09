using Galaxy.Conqueror.Client.Models;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        public static readonly int MENU_MARGIN = int.Parse(ConfigurationManager.AppSettings.Get("MENU_MARGIN") ?? "0");
        public static int canvasWidth = MAP_WIDTH* 2 + MENU_WIDTH;
        public static int canvasHeight = MAP_HEIGHT;
        public static  GameState state = GameState.RUNNING;
        public static int playerShipID = 1;


    }
}
