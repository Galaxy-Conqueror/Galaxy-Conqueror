using Galaxy.Conqueror.Client;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.GameModels;
using System;
using System.Collections.Generic;
using System.Threading;

public class MapView
{
    private static readonly char[,] map = new char[StateManager.MAP_WIDTH,  StateManager.MAP_HEIGHT];

    private static String inputQueue = "";

    public static bool stale = true;

    // Mapping display symbols to colors
    private static readonly Dictionary<char, string> colorMap = new()
    {
        { '*', ConsolePrinter.MAGENTA },  // Generic planet
        { 'P', ConsolePrinter.CYAN },     // Your planet (cyan *)
        { 'V', ConsolePrinter.YELLOW },   // Spaceship
        { '.', ConsolePrinter.WHITE }     // Empty space
    };

    public static void InitialiseMap()
    {
        InitMap();

        stale = true;
    }

    public static char[,] GetMap()
    {
        return map;
    }

    private static void InitMap()
    {
        for (int y = 0; y <  StateManager.MAP_HEIGHT; y++)
            for (int x = 0; x <  StateManager.MAP_WIDTH; x++)
                map[x, y] = '.';
    }
}


