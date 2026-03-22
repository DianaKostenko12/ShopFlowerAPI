using DAL.Models;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.Dto
{
    public sealed record BouquetDetails
    (
        string BouquetName,
        List<FlowerComposition.Dto.FlowerComposition> FlowerComposition,
        WrappingPaper WrappingPaper,
        string Shape
    );
}
