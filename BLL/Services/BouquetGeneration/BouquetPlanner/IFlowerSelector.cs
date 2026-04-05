using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.OpenAi.Dto;

namespace BLL.Services.BouquetGeneration.BouquetPlanner
{
    public interface IFlowerSelector
    {
        Task<List<FlowerWithRole>> SelectFlowersWithRolesAsync(
            GptStyleRecommendation aiStyleAdvice,
            IReadOnlyCollection<ColorPreference> primaryColors,
            IReadOnlyCollection<ColorPreference> accentColors);
    }
}
