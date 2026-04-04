namespace BLL.Services.BouquetAssembly.DTOs
{
    /// <summary>
    /// Проміжний результат від стратегії розміщення квітів (без обгортки).
    /// </summary>
    public record LayoutResult
    (
        bool IsAssembled,
        DateTime CompletionTime,
        string AssemblyType,
        double FinalWidthCm,
        List<FlowerCoordinate> Coordinates
    );
}
