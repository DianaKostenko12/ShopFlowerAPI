using DAL.Models;
using DAL.Models.Flowers;

namespace FlowerShopApi.DTOs.AIGeneratedBouquets
{
    public record BouquetCompositionItem
    (
        int FlowerId,
        FlowerRole FlowerRole,
        int Quantity
    );
}
