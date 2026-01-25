using System.Text.Json.Serialization;

namespace BLL.Services.OpenAi.Dto
{
    public sealed class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; init; } = "user";

        [JsonPropertyName("content")]
        public List<MessageContent> Content { get; init; } = [];
    }
}
