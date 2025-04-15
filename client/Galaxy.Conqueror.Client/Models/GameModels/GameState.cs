using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models.GameModels;

public enum GameState
{
    MAP_VIEW,
    QUIT_REQUESTED,
    IDLE,
    PLANET_MANAGEMENT,
    BATTLE,
}
