using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.Colors;
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
        private readonly IColorService _colorService;

        public OpenAIService(IOpenAiClient openAiClient, IColorService colorService)
        {
            _openAiClient = openAiClient;
            _colorService = colorService;
        }

        public async Task<GptStyleRecommendation> GenerateBouquetDescriptionAsync(GenerateBouquetDescriptor descriptor, CancellationToken cancellationToken = default)
        {
            var allowedShades = (await _colorService.GetColorsAsync())
                .Select(color => color.Shade?.Trim())
                .Where(shade => !string.IsNullOrWhiteSpace(shade))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(shade => shade)
                .ToList();

            var prompt = PromptBuilder.BuildStylePrompt(descriptor, allowedShades);

            var response = await _openAiClient.GenerateTextAsync(prompt, "text");

            return JsonSerializer.Deserialize<GptStyleRecommendation>(response,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<byte[]> GenerateBouquetImageAsync(BouquetDetails bouquetDetails, CancellationToken cancellationToken = default)
        {
            var prompt = PromptBuilder.BuildImagePrompt(bouquetDetails);

            return await _openAiClient.GenerateImageAsync(prompt, "image");
        }
    }
}

