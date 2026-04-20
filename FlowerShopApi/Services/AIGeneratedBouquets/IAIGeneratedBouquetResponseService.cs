using BLL.Services.BouquetGeneration.Responses;
using FlowerShopApi.DTOs.AIGeneratedBouquets;

namespace FlowerShopApi.Services.AIGeneratedBouquets
{
    public interface IAIGeneratedBouquetResponseService
    {
        GenerateAIBouquetResponse BuildResponse(GenerateBouquetResponse generatedBouquet);
    }
}
