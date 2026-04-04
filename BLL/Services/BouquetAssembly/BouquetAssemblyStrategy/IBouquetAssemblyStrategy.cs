using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;

namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    public interface IBouquetAssemblyStrategy
    {
        LayoutResult AssembleBouquet(List<ProcessedFlower> processedFlowersForAssembly);
    }
}
