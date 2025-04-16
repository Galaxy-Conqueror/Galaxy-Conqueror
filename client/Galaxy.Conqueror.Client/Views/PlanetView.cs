using Galaxy.Conqueror.Client;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

public class PlanetView
{
    private static Dictionary<Vector2I, Glyph> map { get; set; } = new();
    private static readonly Dictionary<PlanetTypes, Dictionary<Vector2I, Glyph>> planets = new();
    public static bool Stale = true;

    public static void Initialise()
    {
        LoadPlanetFromFile("PlanetAscii\\planet_water.json");
        planets.Add(PlanetTypes.Water, map);

        LoadPlanetFromFile("PlanetAscii\\desert.json");
        planets.Add(PlanetTypes.Desert, map);

        LoadPlanetFromFile("PlanetAscii\\jungle.json");
        planets.Add(PlanetTypes.Jungle, map);

        LoadPlanetFromFile("PlanetAscii\\ice.json");
        planets.Add(PlanetTypes.Ice, map);

        Stale = true;
    }

    public static Dictionary<Vector2I, Glyph> GetScreen()
    {
        var adjacentEntity = EntityManager.Entities.Where(x => x != StateManager.PlayerSpaceship).FirstOrDefault(x => StateManager.PlayerSpaceship.Position.DistanceTo(x.Position) <= 2);
        Stale = false;

        if (adjacentEntity is Planet adjacentPlanet)
        {
            switch (adjacentPlanet.Design)
            {
                case "Desert":
                    return planets[PlanetTypes.Desert];                
                case "Jungle":
                    return planets[PlanetTypes.Desert];
                case "Water":
                    return planets[PlanetTypes.Water];
                case "Ice":
                    return planets[PlanetTypes.Ice];
                default:
                    return planets[PlanetTypes.Desert];
            }
        }
            
        return planets[PlanetTypes.Desert];
    }

    public static void LoadPlanetAsciiFromJson(string jsonContent)
    {
        try
        {
            var jsonDocument = JsonDocument.Parse(jsonContent);
            var root = jsonDocument.RootElement;
            map = new();

            if (root.TryGetProperty("picture", out JsonElement pictureElement) &&
                pictureElement.ValueKind == JsonValueKind.Array)
            {
                int arrayLength = pictureElement.GetArrayLength();

                map.Clear();

                for (int i = 0; i < arrayLength; i++)
                {
                    string line = pictureElement[i].GetString();
                    if (line == null) continue;

                    ProcessAsciiLine(line, i);
                }

                Stale = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
        }
    }

    public static void AddPlayerShip(Vector2I origin)
    {
        if (string.IsNullOrEmpty(StateManager.PlayerSpaceship.Design))
            return;

        string[] rectangle = StateManager.PlayerSpaceship.Design.Split("\r\n");

        for (int y = 0; y < rectangle.Length; y++)
        {
            string line = rectangle[y];
            for (int x = 0; x < line.Length; x++)
            {
                char c = line[x];
                Vector2I position = new Vector2I(origin.X + x, origin.Y + y);

                if (c != 'S')
                {
                    if (map.ContainsKey(position))
                    {
                        map.Remove(position);
                    }
                    map.Add(position, new Glyph(c, StateManager.PlayerSpaceship.Glyph.Color));
                }
                else
                {
                    if (map.ContainsKey(position))
                    {
                        map.Remove(position);
                    }
                }
            }
        }   
    }

    private static void ProcessAsciiLine(string line, int y)
    {
        var extractedData = ExtractCharactersAndColors(line);
        string plainText = extractedData.Item1;
        Dictionary<int, string> colorMap = extractedData.Item2;

        plainText = plainText.Replace("</color>", "");
        

        for (int i = 0; i < plainText.Length; i++)
        {
            int x = i;

            var glyph = plainText[i];

            if (glyph == ' ')
            {
                continue;
            }

            if (x >= 0 && x < StateManager.MAP_WIDTH &&
                y >= 0 && y < StateManager.MAP_HEIGHT)
            {
                Vector2I position = new Vector2I(x, y);
                if (map.ContainsKey(position))
                {
                    map.Remove(position);
                }

                ConsoleColor color = ConsoleColor.White;
                if (colorMap.TryGetValue(i, out string colorValue))
                {
                    if (Enum.TryParse(colorValue, true, out ConsoleColor parsedColor))
                    {
                        color = parsedColor;
                    }
                }

                map.Add(position, new Glyph(glyph, color));
            }
        }
    }

    private static Tuple<string, Dictionary<int, string>> ExtractCharactersAndColors(string input)
    {
        string plainText = "";
        Dictionary<int, string> colorMap = new Dictionary<int, string>();

        int currentPlainTextIndex = 0;
        int inputIndex = 0;

        while (inputIndex < input.Length)
        {
            int colorTagStartIndex = input.IndexOf("<color_", inputIndex);

            if (colorTagStartIndex == -1)
            {
                plainText += input.Substring(inputIndex);
                break;
            }

            if (colorTagStartIndex > inputIndex)
            {
                string textBeforeTag = input.Substring(inputIndex, colorTagStartIndex - inputIndex);
                plainText += textBeforeTag;
                currentPlainTextIndex += textBeforeTag.Length;
            }

            int colorNameEndIndex = input.IndexOf(">", colorTagStartIndex);
            if (colorNameEndIndex == -1) break;

            string colorName = input.Substring(colorTagStartIndex + 7, colorNameEndIndex - (colorTagStartIndex + 7));

            int closingTagIndex = input.IndexOf("</color>", colorNameEndIndex);
            if (closingTagIndex == -1)
            {
                string color = input.Substring(colorNameEndIndex + 1);
                for (int i = 0; i < color.Length; i++)
                {
                    plainText += color[i];
                    colorMap[currentPlainTextIndex + i] = colorName;
                }
                break;
            }

            string colorContent = input.Substring(colorNameEndIndex + 1, closingTagIndex - (colorNameEndIndex + 1));

            for (int i = 0; i < colorContent.Length; i++)
            {
                plainText += colorContent[i];
                colorMap[currentPlainTextIndex + i] = colorName;
            }

            currentPlainTextIndex += colorContent.Length;
            inputIndex = closingTagIndex + "</color>".Length;
        }

        return new Tuple<string, Dictionary<int, string>>(plainText, colorMap);
    }

    public static void LoadPlanetFromFile(string filePath)
    {
        try
        {
            string jsonContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath));

            LoadPlanetAsciiFromJson(jsonContent);
            AddPlayerShip(new Vector2I((StateManager.MAP_SCREEN_WIDTH) - 8, StateManager.MAP_SCREEN_HEIGHT - 8));
        }
        catch (Exception ex)
        {
            
        }
    }
}