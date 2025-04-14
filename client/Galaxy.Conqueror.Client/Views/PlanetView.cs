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
    private static readonly Dictionary<Vector2I, char> map = new();
    private static readonly Dictionary<string, Dictionary<Vector2I, char>> planets = new();
    public static bool Stale = true;

    public static void Initialise()
    {
        //LoadPlanetFromFile("C:\\Users\\bbdnet2817\\OneDrive - BBD Software Development\\BBD\\grad_program\\C#\\Galaxy-Conqueror\\client\\Galaxy.Conqueror.Client\\PlanetAscii\\blank-planet.json");

        LoadPlanetFromJsonString();

        //InitMap();

        planets.Add("blank", map);

        Stale = true;
    }

    public static Dictionary<Vector2I, char> GetScreen()
    {
        Stale = false;
        return planets["blank"];
    }

    private static void InitMap()
    {
        const double starDensity = 0.05;
        Random rand = new();

        for (int y = 0; y < StateManager.MAP_HEIGHT; y++)
        {
            for (int x = 0; x < StateManager.MAP_WIDTH; x++)
            {
                //if (x == 0 || x == StateManager.MAP_WIDTH - 1 || y == 0 || y == StateManager.MAP_HEIGHT - 1)
                //{
                //    map.Add(new Vector2I(x, y), '#');
                //}
                //if (rand.Next(1, 100) / 100.0 < starDensity)
                //{
                map.Add(new Vector2I(x, y), '.');
                //}
            }
        }
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
                int centerX = StateManager.MAP_WIDTH / 2;
                int centerY = StateManager.MAP_HEIGHT / 2;
                int arrayLength = pictureElement.GetArrayLength();
                int halfHeight = arrayLength / 2;

                for (int i = 0; i < arrayLength; i++)
                {
                    string line = pictureElement[i].GetString();
                    if (line == null) continue;

                    // Process the line to extract characters and colors
                    ProcessAsciiLine(line, i, centerX, centerY, halfHeight);
                }

                Stale = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
        }
    }

    private static void ProcessAsciiLine(string line, int lineIndex, int centerX, int centerY, int halfHeight)
    {
        // Extract characters and colors from the line
        var extractedData = ExtractCharactersAndColors(line);
        string plainText = extractedData.Item1;
        Dictionary<int, string> colorMap = extractedData.Item2;

        int lineLength = plainText.Length;
        int startX = centerX - (lineLength / 2);
        int y = centerY - halfHeight + lineIndex;

        for (int i = 0; i < plainText.Length; i++)
        {
            int x = startX + i;

            // Skip spaces or add only non-space characters
            if (plainText[i] != ' ' &&
                x >= 0 && x < StateManager.MAP_WIDTH &&
                y >= 0 && y < StateManager.MAP_HEIGHT)
            {
                // If position already has something, remove it first
                Vector2I position = new Vector2I(x, y);
                if (map.ContainsKey(position))
                {
                    map.Remove(position);
                }

                map.Add(position, plainText[i]);

                // Here's where you'd use the color if needed
                string color = "";
                if (colorMap.TryGetValue(i, out string colorValue))
                {
                    color = colorValue;
                    // For now, just storing the color in an unused variable as requested
                }
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

            // No more color tags found
            if (colorTagStartIndex == -1)
            {
                plainText += input.Substring(inputIndex);
                break;
            }

            // Add text before the color tag
            if (colorTagStartIndex > inputIndex)
            {
                string textBeforeTag = input.Substring(inputIndex, colorTagStartIndex - inputIndex);
                plainText += textBeforeTag;
                currentPlainTextIndex += textBeforeTag.Length;
            }

            // Find the end of the color name
            int colorNameEndIndex = input.IndexOf(">", colorTagStartIndex);
            if (colorNameEndIndex == -1) break;

            // Extract the color name
            string colorName = input.Substring(colorTagStartIndex + 7, colorNameEndIndex - (colorTagStartIndex + 7));

            // Find the closing color tag
            int closingTagIndex = input.IndexOf("</color>", colorNameEndIndex);
            if (closingTagIndex == -1) break;

            // Extract the content between the color tags
            string colorContent = input.Substring(colorNameEndIndex + 1, closingTagIndex - (colorNameEndIndex + 1));

            // Add the colored content to the plainText and record the color in the map
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

    // Method to load planet ASCII art from a file path
    public static void LoadPlanetFromFile(string filePath)
    {
        try
        {
            string jsonContent = File.ReadAllText(filePath);
            LoadPlanetAsciiFromJson(jsonContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }
    }

    // Method to load planet ASCII art from a JSON string
    public static void LoadPlanetFromJsonString()
    {
        var jsonString = "  {\r\n    \"type\": \"ascii_art\",\r\n    \"id\": \"blank planet\",\r\n    \"picture\": [\r\n      \"                         <color_white>∙∙∙∙∙∙∙∙∙∙</color>                         \",\r\n      \"                     <color_white>∙∙∙∙</color>          <color_white>∙∙∙∙</color>                     \",\r\n      \"                  <color_white>∙∙∙</color>                  <color_white>∙∙∙</color>                  \",\r\n      \"                <color_white>∙∙</color>                        <color_white>∙∙</color>                \",\r\n      \"              <color_white>∙∙</color>                            <color_white>∙∙</color>              \",\r\n      \"             <color_white>∙</color>                                <color_white>∙</color>             \",\r\n      \"            <color_white>∙</color>                                  <color_white>∙</color>            \",\r\n      \"          <color_white>∙∙</color>                                    <color_white>∙∙</color>          \",\r\n      \"         <color_white>∙</color>                                        <color_white>∙</color>         \",\r\n      \"        <color_white>∙</color>                                          <color_white>∙</color>        \",\r\n      \"       <color_white>∙</color>                                            <color_white>∙</color>       \",\r\n      \"       <color_white>∙</color>                                            <color_white>∙</color>       \",\r\n      \"      <color_white>∙</color>                                              <color_white>∙</color>      \",\r\n      \"     <color_white>∙</color>                                                <color_white>∙</color>     \",\r\n      \"    <color_white>∙</color>                                                  <color_white>∙</color>    \",\r\n      \"    <color_white>∙</color>                                                  <color_white>∙</color>    \",\r\n      \"   <color_white>∙</color>                                                    <color_white>∙</color>   \",\r\n      \"   <color_white>∙</color>                                                    <color_white>∙</color>   \",\r\n      \"  <color_white>∙</color>                                                      <color_white>∙</color>  \",\r\n      \"  <color_white>∙</color>                                                      <color_white>∙</color>  \",\r\n      \"  <color_white>∙</color>                                                      <color_white>∙</color>  \",\r\n      \" <color_white>∙</color>                                                        <color_white>∙</color> \",\r\n      \" <color_white>∙</color>                                                        <color_white>∙</color> \",\r\n      \" <color_white>∙</color>                                                        <color_white>∙</color> \",\r\n      \" <color_white>∙</color>                                                        <color_white>∙</color> \",\r\n      \"<color_white>∙</color>                                                          <color_white>∙\",\r\n      \"∙</color>                                                          <color_white>∙\",\r\n      \"∙</color>                                                          <color_white>∙\",\r\n      \"∙</color>                                                          <color_white>∙\",\r\n      \"∙</color>                                                          <color_white>∙\",\r\n      \"∙</color>                                                          <color_white>∙\",\r\n      \"∙</color>                                                          <color_white>∙\",\r\n      \"∙</color>                                                          <color_white>∙\",\r\n      \"∙</color>                                                          <color_white>∙\",\r\n      \"∙</color>                                                          <color_white>∙\",\r\n      \"</color> <color_white>∙</color>                                                        <color_white>∙</color> \",\r\n      \" <color_white>∙</color>                                                        <color_white>∙</color> \",\r\n      \" <color_white>∙</color>                                                        <color_white>∙</color> \",\r\n      \" <color_white>∙</color>                                                        <color_white>∙</color> \",\r\n      \"  <color_white>∙</color>                                                      <color_white>∙</color>  \",\r\n      \"  <color_white>∙</color>                                                      <color_white>∙</color>  \",\r\n      \"  <color_white>∙</color>                                                      <color_white>∙</color>  \",\r\n      \"   <color_white>∙</color>                                                    <color_white>∙</color>   \",\r\n      \"   <color_white>∙</color>                                                    <color_white>∙</color>   \",\r\n      \"    <color_white>∙</color>                                                  <color_white>∙</color>    \",\r\n      \"    <color_white>∙</color>                                                  <color_white>∙</color>    \",\r\n      \"     <color_white>∙</color>                                                <color_white>∙</color>     \",\r\n      \"      <color_white>∙</color>                                              <color_white>∙</color>      \",\r\n      \"       <color_white>∙</color>                                            <color_white>∙</color>       \",\r\n      \"       <color_white>∙</color>                                            <color_white>∙</color>       \",\r\n      \"        <color_white>∙</color>                                          <color_white>∙</color>        \",\r\n      \"         <color_white>∙</color>                                        <color_white>∙</color>         \",\r\n      \"          <color_white>∙∙</color>                                    <color_white>∙∙</color>          \",\r\n      \"            <color_white>∙</color>                                  <color_white>∙</color>            \",\r\n      \"             <color_white>∙</color>                                <color_white>∙</color>             \",\r\n      \"              <color_white>∙∙</color>                            <color_white>∙∙</color>              \",\r\n      \"                <color_white>∙∙</color>                        <color_white>∙∙</color>                \",\r\n      \"                  <color_white>∙∙∙</color>                  <color_white>∙∙∙</color>                  \",\r\n      \"                     <color_white>∙∙∙∙</color>          <color_white>∙∙∙∙</color>                     \",\r\n      \"                         <color_white>∙∙∙∙∙∙∙∙∙∙</color>                         \"\r\n    ]\r\n  }\r\n";

        LoadPlanetAsciiFromJson(jsonString);
    }
}