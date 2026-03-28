using BLL.Services.BouquetAssembly.DTOs;

namespace BLL.Services.BouquetAssembly.Descriptors
{
    public record AssemblyPlanDescriptor
    (
        int BouquetId,
        string Shape,
        int WrappingPaperId,
        List<AssemblyFlowerItem> Flowers
    );
}
