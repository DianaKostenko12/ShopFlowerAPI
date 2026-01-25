using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.BouquetGeneration.Responses;

namespace BLL.Services.BouquetGeneration
{
    internal class BouquetGenerationService : IBouquetGenerationService
    {
        public Task<GenerateBouquetResponse> GenerateBouquetAsync(GenerateBouquetDescriptor descriptor, CancellationToken cancellationToken = default)
        {
            
        }
    }
}
