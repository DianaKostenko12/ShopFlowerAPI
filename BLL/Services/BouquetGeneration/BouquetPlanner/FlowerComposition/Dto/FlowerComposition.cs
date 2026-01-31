using DAL.Models;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition.Dto
{
    public sealed record FlowerComposition(
      Flower flower,
      string Role,
      int Quantity,
      decimal UnitPrice
    );
}
