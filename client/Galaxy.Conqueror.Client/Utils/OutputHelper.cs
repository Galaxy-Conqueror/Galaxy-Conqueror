namespace Galaxy.Conqueror.Client.Utils
{
    public static class OutputHelper
    {
        private static readonly bool DEBUG = true;

        public static void DebugPrint(string output)
        {
            if (DEBUG)
            {
                Console.WriteLine("DEBUG: " + output);
            }
        }
    }
}
