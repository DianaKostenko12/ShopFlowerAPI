using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.Colors;
using BLL.Services.Flowers;
using BLL.Services.OpenAi.Dto;
using DAL.Models;

namespace BLL.Services.BouquetGeneration.BouquetPlanner
{
    public class FlowerSelector : IFlowerSelector
    {
        private readonly IFlowerService _flowerService;
        private readonly IColorMatchingService _colorMatchingService;

        public FlowerSelector(IFlowerService flowerService, IColorMatchingService colorMatchingService)
        {
            _flowerService = flowerService;
            _colorMatchingService = colorMatchingService;
        }

        public async Task<List<FlowerWithRole>> SelectFlowersWithRolesAsync(
            GptStyleRecommendation aiStyleAdvice,
            IReadOnlyCollection<ColorPreference> primaryColors,
            IReadOnlyCollection<ColorPreference> accentColors)
        {
            var flowers = await _flowerService.GetFlowersAsync();

            var filteredFlowers = flowers.Where(flower =>
                    aiStyleAdvice.Roles.Focal.Categories.Contains(flower.Category?.CategoryName) ||
                    aiStyleAdvice.Roles.Semi.Categories.Contains(flower.Category?.CategoryName) ||
                    aiStyleAdvice.Roles.Filler.Categories.Contains(flower.Category?.CategoryName) ||
                    aiStyleAdvice.Roles.Greenery.Categories.Contains(flower.Category?.CategoryName)
                )
                .Where(f =>
                    _colorMatchingService.ResolveColorScore(f.Color, primaryColors, 0.4).HasValue ||
                    _colorMatchingService.ResolveColorScore(f.Color, accentColors, 0.2).HasValue
                )
                .ToList();

            return filteredFlowers.Select(flower =>
            {
                var role =
                    aiStyleAdvice.Roles.Focal.Categories.Contains(flower.Category?.CategoryName)
                        ? FlowerRole.Focal
                        : aiStyleAdvice.Roles.Greenery.Categories.Contains(flower.Category?.CategoryName)
                        ? FlowerRole.Greenery
                        : aiStyleAdvice.Roles.Semi.Categories.Contains(flower.Category?.CategoryName)
                        ? FlowerRole.Semi
                        : FlowerRole.Filler;

                return new FlowerWithRole(
                    role,
                    flower,
                    CalculateHarmony(flower, role, primaryColors, accentColors)
                );
            }).ToList();
        }

        private double CalculateHarmony(
            DAL.Models.Flowers.Flower flower,
            FlowerRole role,
            IReadOnlyCollection<ColorPreference> primaryColors,
            IReadOnlyCollection<ColorPreference> accentColors)
        {
            double harmony = 1.0;

            harmony += _colorMatchingService.ResolveColorScore(flower.Color, primaryColors, 0.4)
                    ?? _colorMatchingService.ResolveColorScore(flower.Color, accentColors, 0.2)
                    ?? 0;

            harmony += role switch
            {
                FlowerRole.Focal => 0.4,
                FlowerRole.Filler => 0.2,
                FlowerRole.Semi => 0.3,
                FlowerRole.Greenery => 0.1,
                _ => 0
            };

            return harmony;
        }
    }
}
