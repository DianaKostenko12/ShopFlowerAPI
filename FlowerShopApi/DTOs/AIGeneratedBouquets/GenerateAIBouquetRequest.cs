namespace FlowerShopApi.DTOs.AIGeneratedBouquets
{
    public record GenerateAIBouquetRequest
    (
         List<RequestedBouquetColorDto> Color,
         decimal Budget,
         string Style,
         string Shape,
         string AdditionalComment
    );
}
