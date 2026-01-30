using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.Flowers;
using BLL.Services.OpenAi;
using BLL.Services.OpenAi.Dto;
using DAL.Models;

namespace BLL.Services.BouquetGeneration.BouquetPlanner
{
    internal class BouquetPlanner : IBouquetPlanner
    {
        private readonly IOpenAIService _openAIService;
        private readonly IFlowerService _flowerService;
        public BouquetPlanner(IOpenAIService openAiService, IFlowerService flowerService)
        {
            _openAIService = openAiService;
            _flowerService = flowerService;
        }
        public async Task<BouquetAssemblyPlan> BuildPlanAsync(GenerateBouquetDescriptor descriptor, CancellationToken ct)
        {
            var aiStyleAdvice = await _openAIService.GenerateBouquetDescriptionAsync(descriptor);

            var flowers = _flowerService.GetFlowersAsync();

            var filteredFlowers = flowers.Result.Where(flower =>
                aiStyleAdvice.Roles.Focal.Categories.Contains(flower.Category) ||
                aiStyleAdvice.Roles.Semi.Categories.Contains(flower.Category) ||
                aiStyleAdvice.Roles.Filler.Categories.Contains(flower.Category) ||
                aiStyleAdvice.Roles.Greenery.Categories.Contains(flower.Category)
            )
            .Where(f =>
                descriptor.Color.Contains(f.Color) ||
                aiStyleAdvice.Palette.Primary.Contains(f.Color) ||
                aiStyleAdvice.Palette.Accent.Contains(f.Color)
            )
            .ToList();

            var flowersWithRole = filteredFlowers.Select(flower =>
            {
                var role =
                    aiStyleAdvice.Roles.Focal.Categories.Contains(flower.Category)
                        ? RolesConstants.FocalCategory
                        : aiStyleAdvice.Roles.Greenery.Categories.Contains(flower.Category)
                        ? RolesConstants.GreeneryCategory
                        : aiStyleAdvice.Roles.Semi.Categories.Contains(flower.Category)
                        ? RolesConstants.SemiCategory
                        : RolesConstants.FillerCategory;
                return new FlowerWithRole(
                    role, 
                    flower, 
                    CalculateHarmony(flower, role, aiStyleAdvice)
                );
            }).ToList();

        }

        private double CalculateHarmony(
            Flower flower,
            string role,
            GptStyleRecommendation aiStyleAdvice
        )
        {
            double harmony = 1.0;

            harmony += aiStyleAdvice.Palette.Primary.Contains(flower.Color) 
                    ? 0.4
                    : aiStyleAdvice.Palette.Accent.Contains(flower.Color) 
                    ? 0.2 : 0;

            harmony += role switch
            {
                RolesConstants.FocalCategory => 0.4,
                RolesConstants.FillerCategory => 0.2,
                RolesConstants.SemiCategory => 0.3,
                RolesConstants.GreeneryCategory => 0.1,
                _ => 0
            };

            return harmony;
        }
    }
}
