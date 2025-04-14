using Galaxy.Conqueror.Client;
using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Utils;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Client.Start();

        Console.Clear();
        Console.WriteLine("Goodbye...");
        Thread.Sleep(2000);
    }
}
