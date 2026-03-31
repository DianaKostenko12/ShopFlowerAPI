using BLL.Services.BouquetAssembly.Descriptors;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;
using BLL.Services.BouquetAssembly.Responses;

namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    public interface IBouquetAssemblyStrategy
    {
        AssemblyResult AssembleBouquet(List<ProcessedFlower> processedFlowersForAssembly);
    }
}
