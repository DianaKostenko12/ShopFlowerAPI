namespace BLL.Services.OpenAi.OpenAiClient
{
    internal interface IOpenAiClient
    {
        Task<string> GenerateTextAsync(string prompt, string responseFormat, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateImageAsync(string prompt, string responseFormat, CancellationToken cancellationToken = default);
    }
}