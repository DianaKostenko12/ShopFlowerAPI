using System.Text.Json.Serialization;

namespace BLL.Services.OpenAi.Dto
{
    public sealed class ResponseFormat
    {
        [JsonPropertyName("type")]
        public string Type { get; init; } = default!;
    }
}
