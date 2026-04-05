using DAL.Models;
using DAL.Models.Flowers;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition.Dto
{
    public sealed record FlowerComposition(
      Flower flower,
      FlowerRole Role,
      int Quantity,
      decimal UnitPrice
    );
}
