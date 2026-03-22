namespace BLL.Services.OpenAi.OpenAiClient
{
    public interface IOpenAiClient
    {
        Task<string> GenerateTextAsync(string prompt, string responseFormat, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateImageAsync(string prompt, string responseFormat, CancellationToken cancellationToken = default);
    }
}