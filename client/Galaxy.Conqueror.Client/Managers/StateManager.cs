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

        public static readonly int MAP_SCREEN_WIDTH = int.Parse(ConfigurationManager.AppSettings.Get("MAP_SCREEN_WIDTH") ?? "0");
        public static readonly int MAP_SCREEN_HEIGHT = int.Parse(ConfigurationManager.AppSettings.Get("MAP_SCREEN_HEIGHT") ?? "0");

        public static readonly int MENU_MARGIN = int.Parse(ConfigurationManager.AppSettings.Get("MENU_MARGIN") ?? "0");

        public static int CanvasWidth = (MAP_WIDTH * 2) + MENU_WIDTH + MENU_MARGIN;
        public static int CanvasHeight = MAP_HEIGHT;
        public static  GameState State = GameState.MAP_VIEW;
        public static int PlayerShipID = 1;
    }
}
