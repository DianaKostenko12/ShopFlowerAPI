using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.OpenAi.Dto;
using BLL.Services.OpenAi.OpenAiClient;
using BLL.Services.OpenAi.Utils;
using System.Text.Json;

namespace BLL.Services.OpenAi
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IOpenAiClient _openAiClient;
        public OpenAIService(IOpenAiClient openAiClient)
        {
            _openAiClient = openAiClient;
        }

        public async Task<GptStyleRecommendation> GenerateBouquetDescriptionAsync(GenerateBouquetDescriptor descriptor, CancellationToken cancellationToken = default)
        {
            var prompt = PromptBuilder.BuildStylePrompt(descriptor);

            var response = await _openAiClient.GenerateTextAsync(prompt, "text");

            return JsonSerializer.Deserialize<GptStyleRecommendation>(response);
        }

        public async Task<byte[]> GenerateBouquetImageAsync(BouquetDetails bouquetDetails, CancellationToken cancellationToken = default)
        {
            var prompt = PromptBuilder.BuildImagePrompt(bouquetDetails);

            return await _openAiClient.GenerateImageAsync(prompt, "image");
        }
    }
}
