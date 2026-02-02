using DAL.Models;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.Dto
{
    public sealed record BouquetDetails
    (
        List<FlowerComposition.Dto.FlowerComposition> FlowerComposition,
        WrappingPaper WrappingPaper,
        string Shape
    );
}
