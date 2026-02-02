using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.BouquetGeneration.Descriptors;

namespace BLL.Services.BouquetGeneration.BouquetPlanner
{
    public interface IBouquetPlanner
    {
        Task<BouquetDetails> BuildPlanAsync(GenerateBouquetDescriptor req, CancellationToken ct);
    }
}
