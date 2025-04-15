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
    private static readonly Dictionary<Vector2I, Glyph> map = new();
    private static readonly Dictionary<string, Dictionary<Vector2I, Glyph>> planets = new();
    public static bool Stale = true;

    public static void Initialise()
    {
        LoadPlanetFromFile("C:\\Users\\bbdnet2817\\OneDrive - BBD Software Development\\BBD\\grad_program\\C#\\Galaxy-Conqueror\\client\\Galaxy.Conqueror.Client\\PlanetAscii\\water.json");

        //InitMap();

        planets.Add("blank", map);

        LoadPlanetFromFile("C:\\Users\\bbdnet2817\\OneDrive - BBD Software Development\\BBD\\grad_program\\C#\\Galaxy-Conqueror\\client\\Galaxy.Conqueror.Client\\PlanetAscii\\desert.json");

        planets.Add("desert", map);

        Stale = true;
    }

    public static Dictionary<Vector2I, Glyph> GetScreen()
    {
        Stale = false;
        return planets["desert"];
    }

    public static void LoadPlanetAsciiFromJson(string jsonContent)
    {
        try
        {
            var jsonDocument = JsonDocument.Parse(jsonContent);
            var root = jsonDocument.RootElement;

            if (root.TryGetProperty("picture", out JsonElement pictureElement) &&
                pictureElement.ValueKind == JsonValueKind.Array)
            {
                int arrayLength = pictureElement.GetArrayLength();

                // Clear the existing map
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
        var entity = EntityManager.Entities.First(x => x.Id == StateManager.PlayerShipID);
        if (entity is Spaceship ship)
        {
            if (string.IsNullOrEmpty(ship.Design))
                return;

            string[] rectangle = ship.Design.Split("\r\n");

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
                        map.Add(position, new Glyph(c, ship.Glyph.Color));
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
            string jsonContent = File.ReadAllText(filePath);
            LoadPlanetAsciiFromJson(jsonContent);
            AddPlayerShip(new Vector2I((StateManager.MAP_SCREEN_WIDTH) - 8, StateManager.MAP_SCREEN_HEIGHT - 8));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }
    }
}