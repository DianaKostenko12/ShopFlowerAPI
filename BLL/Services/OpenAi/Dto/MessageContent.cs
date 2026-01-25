using System.Text.Json.Serialization;

namespace BLL.Services.OpenAi.Dto
{
    public sealed class MessageContent
    {
        [JsonPropertyName("type")]
        public string Type { get; init; } = "text";

        [JsonPropertyName("text")]
        public string Text { get; init; } = default!;
    }
}
