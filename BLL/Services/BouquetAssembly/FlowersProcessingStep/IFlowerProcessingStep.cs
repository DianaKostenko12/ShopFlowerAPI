using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;

namespace BLL.Services.BouquetAssembly.FlowerProcessingStep
{
    public interface IFlowerProcessingStep
    {
        ProcessedFlower ProcessFlower(AssemblyFlowerItem flowerItem);
    }
}
