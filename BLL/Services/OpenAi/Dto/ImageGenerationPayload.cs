using System.Text.Json.Serialization;

namespace BLL.Services.OpenAi.Dto
{
    public sealed class ImageGenerationPayload
    {
        [JsonPropertyName("model")]
        public string Model { get; init; } = default!;

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; init; } = [];

        [JsonPropertyName("response_format")]
        public ResponseFormat ResponseFormat { get; init; } = default!;

        [JsonPropertyName("number_of_images")]
        public int NumberOfImages { get; init; } = default!;

        [JsonPropertyName("size")]
        public string Size {  get; init; } = default!;
    }
}
