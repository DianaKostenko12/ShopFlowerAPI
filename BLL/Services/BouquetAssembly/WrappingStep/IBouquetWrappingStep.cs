using BLL.Services.BouquetAssembly.WrappingStep.DTOs;

namespace BLL.Services.BouquetAssembly.WrappingStep
{
    public interface IBouquetWrappingStep
    {
        WrappingResult WrapBouquet(int wrappingPaperId, double bouquetWidthCm);
    }
}
