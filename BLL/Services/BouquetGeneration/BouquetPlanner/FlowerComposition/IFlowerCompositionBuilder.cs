using BLL.Services.OpenAi.Dto;
using DAL.Models;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition
{
    internal interface IFlowerCompositionBuilder
    {
        List<Dto.FlowerComposition> BuildFlowersComposition(
           IEnumerable<Flower> filteredFlowers,
           GptStyleRecommendation aiStyleRecommendation,
           decimal budget
        );
    }
}
