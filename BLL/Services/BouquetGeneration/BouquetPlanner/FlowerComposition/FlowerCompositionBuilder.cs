using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.OpenAi.Dto;
using DAL.Models;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition
{
    internal sealed class FlowerCompositionBuilder : IFlowerCompositionBuilder
    {
        public List<Dto.FlowerComposition> BuildFlowersComposition(IEnumerable<FlowerWithRole> flowersWithRole, GptStyleRecommendation aiStyleRecommendation, decimal totalBudget)
        {
            List<Dto.FlowerComposition> completedСomposition = new();

            var roleBudgetProportions = new Dictionary<string, decimal>
            {
                { RolesConstants.FocalCategory, 0.4m },
                { RolesConstants.SemiCategory, 0.3m },
                { RolesConstants.FillerCategory, 0.2m },
                { RolesConstants.GreeneryCategory, 0.1m }
            };


            foreach (var role in roleBudgetProportions.Keys)
            {
                decimal roleBudget = totalBudget * roleBudgetProportions[role];

                var candidates = flowersWithRole
                    .Where(f => f.Role == role)
                    .OrderByDescending(f => (decimal)f.Harmony / f.Flower.FlowerCost)
                    .ToList();

                var rolePick = PickForRole(candidates, roleBudget);

                completedСomposition.AddRange(rolePick);
            }

            return completedСomposition;

        }

        private 
    }
}
