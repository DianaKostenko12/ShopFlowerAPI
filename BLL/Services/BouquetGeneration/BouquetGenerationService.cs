using BLL.Services.BouquetGeneration.BouquetPlanner;
using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.BouquetGeneration.Responses;
using BLL.Services.OpenAi;

namespace BLL.Services.BouquetGeneration
{
    public class BouquetGenerationService : IBouquetGenerationService
    {
        private readonly IBouquetPlanner _bouquetPlanner;
        private readonly IOpenAIService _openAIService;
        public BouquetGenerationService(IBouquetPlanner bouquetPlanner, IOpenAIService openAIService)
        {
            _bouquetPlanner = bouquetPlanner;
            _openAIService = openAIService;
        }

        public Task<GenerateBouquetResponse> GenerateBouquetAsync(GenerateBouquetDescriptor descriptor, CancellationToken cancellationToken = default)
        {
            var completedBouquetInfo = _bouquetPlanner.BuildPlanAsync(descriptor, cancellationToken);

            var generatedBouquetImage = _openAIService.GenerateBouquetImageAsync(completedBouquetInfo.Result, cancellationToken);
        }
    }
}
