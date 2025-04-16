namespace Galaxy.Conqueror.API.Services
{
    public interface IAiService
    {
        Task<string> AiGeneratorAsync(string prompt, int maxTokens);
    }
}
