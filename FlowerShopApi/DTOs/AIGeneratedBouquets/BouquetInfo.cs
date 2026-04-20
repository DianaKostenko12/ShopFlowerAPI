namespace FlowerShopApi.DTOs.AIGeneratedBouquets
{
    public record BouquetInfo
    (
        string BouquetName,
        string BouquetDescription,
        List<BouquetCompositionItem> BouquetComposition,
        int WrappingPaperId,
        string Shape
    );
}
