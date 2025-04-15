namespace Galaxy.Conqueror.API.Services;
using Anthropic;


public class AiService(
    IConfiguration configuration, 
    IHostEnvironment env)
{
    public async Task<string> AiGeneratorAsync(string prompt)
    {
        try {
            string apiKey = env.IsDevelopment() 
                ? configuration["AntrhopicApiKey"] ?? "" 
                : Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "";
            using var client = new AnthropicClient(apiKey);

            var messageParams = new CreateMessageParams()
            {   
                Model = new Model(ModelVariant6.Claude35SonnetLatest),
                Messages = [new InputMessage(InputMessageRole.User, prompt)],
                MaxTokens = 500,
                Temperature = 1
            };
            
            var response = await client.Messages.MessagesPostAsync(messageParams);
            var reply = response.Content.OfType<ContentBlock3>().FirstOrDefault().Match(text => text?.Text ?? "");
            return reply ?? "";

        } catch (Exception ex) {
            Console.WriteLine($"Error requesting from AI: {ex}");
            return "";
        }
    }   
}
