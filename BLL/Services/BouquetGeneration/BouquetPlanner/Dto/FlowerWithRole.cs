using DAL.Models;
using DAL.Models.Flowers;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.Dto
{
    public sealed record FlowerWithRole(
        FlowerRole Role,
        Flower Flower,
        double Harmony
    );
}
