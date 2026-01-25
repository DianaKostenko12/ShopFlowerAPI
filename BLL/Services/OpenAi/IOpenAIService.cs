using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.OpenAi.Dto;
namespace BLL.Services.OpenAi
{
    internal interface IOpenAIService
    {
        Task<GptStyleRecommendation> GenerateBouquetDescriptionAsync(
        GenerateBouquetDescriptor descriptor,
        CancellationToken cancellationToken = default);

        Task<byte[]> GenerateBouquetImageAsync(
            string imagePrompt,
            CancellationToken cancellationToken = default);
    }
}
