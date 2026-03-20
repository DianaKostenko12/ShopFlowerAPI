using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
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
            BouquetDetails bouquetDetails,
            CancellationToken cancellationToken = default);
    }
}
