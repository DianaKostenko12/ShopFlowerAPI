namespace BLL.Services.BouquetAssembly.Responses
{
    public record AssemblyResult
    (
        bool IsAssembled,
        DateTime CompletionTime,
        double FinalWidthCm,
        double FinalHeightCm,
        string AssemblyType
    );
}
