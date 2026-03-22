using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.OpenAi.Dto;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition
{
    public interface IFlowerCompositionBuilder
    {
        List<Dto.FlowerComposition> BuildFlowersComposition(
           List<FlowerWithRole> filteredFlowers,
           GptStyleRecommendation aiStyleRecommendation,
           decimal totalBudget
        );
    }
}
