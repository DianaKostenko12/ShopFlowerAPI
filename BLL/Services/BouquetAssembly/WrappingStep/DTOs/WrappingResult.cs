namespace BLL.Services.BouquetAssembly.WrappingStep.DTOs
{
    /// <summary>
    /// Результат етапу обгортання букета.
    /// </summary>
    public record WrappingResult(
        int WrappingPaperId,
        int PaperPatchesUsed,
        int RibbonTiesCount,
        double TotalRibbonLengthCm,
        double BouquetPerimeterCm,
        List<WrappingAction> Actions
    );

    /// <summary>
    /// Одна дія робота під час обгортання (для логування та візуалізації).
    /// </summary>
    public record WrappingAction(
        int StepNumber,
        string ActionType,
        string Description
    );
}
