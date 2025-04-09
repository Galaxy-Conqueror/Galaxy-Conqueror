using System;

public static class ConsolePrinter
{
    public const string RESET = "\u001B[0m";
    public const string BOLD = "\u001B[1m";
    public const string RED = "\u001B[31m";
    public const string GREEN = "\u001B[32m";
    public const string YELLOW = "\u001B[33m";
    public const string BLUE = "\u001B[34m";
    public const string MAGENTA = "\u001B[35m";
    public const string CYAN = "\u001B[36m";
    public const string WHITE = "\u001B[37m";

    public static void Print(string text, params string[] styles)
    {
        string style = string.Join("", styles);
        Console.Write(style + text + RESET);
    }

    public static void PrintLine(string text, params string[] styles)
    {
        Print(text + "\n", styles);
    }

    public static void BlankLine()
    {
        Console.WriteLine();
    }
}
