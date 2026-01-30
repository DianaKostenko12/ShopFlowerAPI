namespace BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition.Dto
{
    public sealed record FlowerComposition(
      int FlowerId,
      string FlowerName,
      string Role,
      int Quantity,
      decimal UnitPrice
    );
}
