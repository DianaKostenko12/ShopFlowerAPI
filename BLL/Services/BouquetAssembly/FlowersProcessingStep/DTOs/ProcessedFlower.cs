using BLL.Services.BouquetAssembly.DTOs;

namespace BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs
{
    public record ProcessedFlower(
        AssemblyFlowerItem Item,
        double GripForce,
        int FinalLength,
        double CutAngle,
        double FinalWeight,
        bool IsStemCleaned
    );
}
