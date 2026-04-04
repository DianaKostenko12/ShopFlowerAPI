using BLL.Services.BouquetAssembly.WrappingStep.DTOs;

namespace BLL.Services.BouquetAssembly.DTOs
{
    public record AssemblyResult
    (
        bool IsAssembled,
        DateTime CompletionTime,
        string AssemblyType,
        double FinalWidthCm,
        List<FlowerCoordinate> Coordinates,
        WrappingResult Wrapping
    );
}
