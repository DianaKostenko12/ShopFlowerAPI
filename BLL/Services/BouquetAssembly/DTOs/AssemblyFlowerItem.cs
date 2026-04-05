using DAL.Models;
using DAL.Models.Flowers;

namespace BLL.Services.BouquetAssembly.DTOs
{
    public record AssemblyFlowerItem
    (
        int FlowerId,
        string FlowerName,
        int Quantity,
        int HeadSizeCm,
        double StemThicknessMm,
        StemType StemKind,
        FlowerRole Role
    );
}
