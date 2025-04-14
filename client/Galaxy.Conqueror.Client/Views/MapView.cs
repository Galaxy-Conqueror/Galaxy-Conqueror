using Galaxy.Conqueror.Client;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

public class MapView : IView
{
    private static readonly Dictionary<Vector2I, Glyph> map = new();

    public static bool stale = true;

    public static void Initialise()
    {
        InitMap();

        stale = true;
    }

    public static Dictionary<Vector2I, Glyph> GetScreen()
    {
        return map;
    }

    private static void InitMap()
    {
        const double starDensity = 0.05;
        Random rand = new();

        for (int y = 0; y < StateManager.MAP_HEIGHT; y++)
        {
            for (int x = 0; x < StateManager.MAP_WIDTH; x++)
            {
                if (rand.Next(1, 100) / 100.0 < starDensity)
                {
                    map.Add(new Vector2I(x, y), new Glyph('.', ConsoleColor.White));
                }
            }
        }
    }
}


