namespace BLL.Services.OpenAi.OpenAiClient
{
    internal interface IOpenAiClient
    {
        Task<string> ChatAsync(string prompt, string responseFormat, CancellationToken ct = default);
    }
}