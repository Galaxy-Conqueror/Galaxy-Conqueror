public class Glyph
{
    public char Character { get; set; }
    public ConsoleColor Color { get; set; }

    public Glyph(char character, ConsoleColor color)
    {
        Character = character;
        Color = color;
    }
}
