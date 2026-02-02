using BLL.Services.BouquetGeneration.BouquetPlanner;
using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.BouquetGeneration.Responses;

namespace BLL.Services.BouquetGeneration
{
    public class BouquetGenerationService : IBouquetGenerationService
    {
        private readonly IBouquetPlanner _bouquetPlanner;
        public BouquetGenerationService(IBouquetPlanner bouquetPlanner)
        {
            _bouquetPlanner = bouquetPlanner;
        }

        public Task<GenerateBouquetResponse> GenerateBouquetAsync(GenerateBouquetDescriptor descriptor, CancellationToken cancellationToken = default)
        {
            var completedBouquetInfo = _bouquetPlanner.BuildPlanAsync(descriptor, cancellationToken);


        }
    }
}
