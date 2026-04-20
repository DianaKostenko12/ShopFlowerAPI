namespace FlowerShopApi.DTOs.AIGeneratedBouquets
{
    public record GenerateAIBouquetResponse
    (
        byte[] BouquetImage,
        BouquetInfo BouquetInfo
    );
}
