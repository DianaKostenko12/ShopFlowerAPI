using DAL.Models;
using DAL.Models.Flowers;

namespace FlowerShopApi.DTOs.AIGeneratedBouquets
{
    public record BouquetCompositionItem
    (
        Flower Flower,
        FlowerRole FlowerRole,
        int Quantity
    );
}
