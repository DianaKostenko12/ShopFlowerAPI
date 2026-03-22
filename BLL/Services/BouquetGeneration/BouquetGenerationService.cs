using BLL.Services.BouquetGeneration.BouquetPlanner;
using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.BouquetGeneration.Responses;
using BLL.Services.OpenAi;

namespace BLL.Services.BouquetGeneration
{
    public class BouquetGenerationService(IBouquetPlanner bouquetPlanner, IOpenAIService openAIService) : IBouquetGenerationService
    {
        public async Task<GenerateBouquetResponse> GenerateBouquetAsync(GenerateBouquetDescriptor descriptor, CancellationToken cancellationToken = default)
        {
            var completedBouquetInfo = await bouquetPlanner.BuildPlanAsync(descriptor, cancellationToken);

            var generatedBouquetImage = await openAIService.GenerateBouquetImageAsync(completedBouquetInfo, cancellationToken);

            return new GenerateBouquetResponse(generatedBouquetImage, completedBouquetInfo);
        }
    }
}
