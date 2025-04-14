using System;
using Galaxy.Conqueror.Client.Utils;

public static class ConsolePrinter
{
    public static void PrintGlyph(Glyph glyph)
    {
        Console.ForegroundColor = glyph.Color;
        Console.Write(glyph.Character);
        Console.ResetColor();
    }

    public static void ClearGlyph()
    {
        Console.Write(" ");
    }

    public static void BlankLine()
    {
        Console.WriteLine();
    }
}
