namespace FlowerShopApi.DTOs.AIGeneratedBouquets
{
    public record GenerateAIBouquetRequest
    (
         List<string> Color,
         decimal Budget,
         string Style,
         string Shape,
         string AdditionalComment
    );
}
