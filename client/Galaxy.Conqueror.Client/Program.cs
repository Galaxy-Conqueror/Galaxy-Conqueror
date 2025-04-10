using Galaxy.Conqueror.Client;
using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Utils;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Client.Start();

        Console.Clear();
        Console.WriteLine(" \n\n\n\n\n \t\t\t Goodbye...");
        Thread.Sleep(2000);
    }
}
