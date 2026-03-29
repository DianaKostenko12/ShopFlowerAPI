using DAL.Models.Flowers;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.Dto
{
    public sealed record FlowerWithRole(
        string Role,
        Flower Flower,
        double Harmony
    );
}
