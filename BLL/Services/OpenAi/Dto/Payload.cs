using System.Text.Json.Serialization;

namespace BLL.Services.OpenAi.Dto
{
    public sealed class Payload
    {
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; init; } = [];

        [JsonPropertyName("response_format")]
        public ResponseFormat ResponseFormat { get; init; } = default!;

        [JsonPropertyName("model")]
        public string Model { get; init; } = default!;

        [JsonPropertyName("temperature")]
        public double Temperature { get; init; }

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; init; }

        [JsonPropertyName("top_p")]
        public double TopP { get; init; }

        [JsonPropertyName("frequency_penalty")]
        public double FrequencyPenalty { get; init; }

        [JsonPropertyName("presence_penalty")]
        public double PresencePenalty { get; init; }
    }
}
