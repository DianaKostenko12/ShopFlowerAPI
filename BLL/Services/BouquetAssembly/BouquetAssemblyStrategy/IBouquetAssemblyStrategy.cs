using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;

namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    public interface IBouquetAssemblyStrategy
    {
        AssemblyResult AssembleBouquet(List<ProcessedFlower> processedFlowersForAssembly);
    }
}
