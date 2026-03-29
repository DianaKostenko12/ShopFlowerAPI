using BLL.Services.BouquetAssembly.DTOs;

namespace BLL.Services.BouquetAssembly.FlowerProcessingStep
{
    public interface IFlowerProcessingStep
    {
        AssemblyFlowerItem ProcessFlower(AssemblyFlowerItem flowerItem);
    }
}
