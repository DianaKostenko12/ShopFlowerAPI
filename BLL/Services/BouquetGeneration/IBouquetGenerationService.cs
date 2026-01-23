using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.BouquetGeneration.Responses;

namespace BLL.Services.BouquetGeneration
{
    public interface IBouquetGenerationService
    {
        Task<GenerateBouquetResponse> GenerateBouquetAsync(
        GenerateBouquetDescriptor descriptor,
        CancellationToken cancellationToken = default);
    }
}