using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.BouquetGeneration.Descriptors;

namespace BLL.Services.BouquetGeneration.BouquetPlanner
{
    internal class BouquetPlanner : IBouquetPlanner
    {
        public Task<BouquetAssemblyPlan> PlanAsync(GenerateBouquetDescriptor req, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
