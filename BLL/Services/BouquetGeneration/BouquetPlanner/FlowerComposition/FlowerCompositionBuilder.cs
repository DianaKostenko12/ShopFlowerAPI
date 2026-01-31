using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.OpenAi.Dto;
using DAL.Models;
using System.Collections.Generic;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition
{
    internal sealed class FlowerCompositionBuilder : IFlowerCompositionBuilder
    {
        public List<Dto.FlowerComposition> BuildFlowersComposition(List<FlowerWithRole> flowersWithRole, GptStyleRecommendation aiStyleRecommendation, decimal totalBudget)
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

                var (minQuantity, maxQuantity) = role switch
                {
                    RolesConstants.FocalCategory => (aiStyleRecommendation.Roles.Focal.Min, aiStyleRecommendation.Roles.Focal.Max),
                    RolesConstants.SemiCategory => (aiStyleRecommendation.Roles.Semi.Min, aiStyleRecommendation.Roles.Semi.Max),
                    RolesConstants.FillerCategory => (aiStyleRecommendation.Roles.Filler.Min, aiStyleRecommendation.Roles.Filler.Max),
                    RolesConstants.GreeneryCategory => (aiStyleRecommendation.Roles.Greenery.Min, aiStyleRecommendation.Roles.Greenery.Max),
                    _ => throw new ArgumentOutOfRangeException(nameof(role))
                };

                var flowerCandidates = flowersWithRole
                    .Where(f => f.Role == role)
                    .OrderByDescending(f => (decimal)f.Harmony / f.Flower.FlowerCost)
                .ToList();

                if (!flowerCandidates.Any())
                {
                    continue;
                }

                if(role == RolesConstants.FocalCategory)
                {
                   var focalRoleComposition = ComposeFlowersForFocalRole(flowerCandidates, roleBudget, maxQuantity, minQuantity);
                   completedСomposition.Add(focalRoleComposition);
                }
                else
                {
                    var roleComposition = ComposeFlowersForRole(flowerCandidates, roleBudget, maxQuantity, minQuantity);
                    completedСomposition.AddRange(roleComposition);
                }
            }

            return completedСomposition;
        }

        private List<Dto.FlowerComposition> ComposeFlowersForRole(List<FlowerWithRole> flowers, decimal roleBudget, int maxQuantity, int minQuantity)
        {
            List<Dto.FlowerComposition> completedComposition = new List<Dto.FlowerComposition>();

            var compositionStates = new Dictionary<
                (int Quantity, decimal Cost),
                (double Harmony, Dictionary<int, int> SelectedFlowerQuantities)
            >();

            compositionStates[(0, 0)] = (0, new Dictionary<int, int>());

            foreach (var flowerCandidate in flowers)
            {
                var newCompositionStates = new Dictionary<(int, decimal), (double, Dictionary<int, int>)>(compositionStates);

                foreach (var state in compositionStates)
                {
                    for (int addQuantity = 1; addQuantity <= maxQuantity; addQuantity++)
                    {
                        int newQuantity = state.Key.Quantity + addQuantity;
                        decimal newCost = state.Key.Cost + addQuantity * flowerCandidate.Flower.FlowerCost;

                        if (newQuantity > maxQuantity || newCost > roleBudget)
                        {
                            break;
                        }

                        double newHarmonyScore = state.Value.Harmony + addQuantity * flowerCandidate.Harmony;

                        var newSelectedFlowerQuantities = new Dictionary<int, int>(state.Value.SelectedFlowerQuantities);
                        newSelectedFlowerQuantities[flowerCandidate.Flower.FlowerId] = newSelectedFlowerQuantities.GetValueOrDefault(flowerCandidate.Flower.FlowerId) + addQuantity;

                        var key = (newQuantity, newCost);

                        if (!newCompositionStates.ContainsKey(key) || newCompositionStates[key].Item1 < newHarmonyScore)
                        {
                            newCompositionStates[key] = (newHarmonyScore, newSelectedFlowerQuantities);
                        }
                    }
                }

                compositionStates = newCompositionStates;
            }

            var bestCompositionStates = compositionStates
                .Where(s => s.Key.Quantity >= minQuantity && s.Key.Quantity <= maxQuantity)
                .OrderByDescending(s => s.Value.Harmony)
                .FirstOrDefault();

            if (bestCompositionStates.Value.SelectedFlowerQuantities != null)
            {
                foreach (var (flowerId, quantity) in bestCompositionStates.Value.SelectedFlowerQuantities)
                {
                    var flower = flowers.First(f => f.Flower.FlowerId == flowerId);
                    completedComposition.Add(new Dto.FlowerComposition(
                        flower.Flower,
                        flower.Role,
                        quantity,
                        flower.Flower.FlowerCost * quantity
                    ));
                }
            }

            return completedComposition;
        }

        private Dto.FlowerComposition ComposeFlowersForFocalRole(List<FlowerWithRole> flowers, decimal roleBudget, int maxQuantity, int minQuantity)
        {
            double bestHarmonyScore = -1;
            var roleComposition = new Dto.FlowerComposition(null, null, 0, 0);

            foreach (var flowerCandidate in flowers)
            {
                int maxQuantityByBudget = (int)(roleBudget / flowerCandidate.Flower.FlowerCost);
                int maxQty = Math.Min(maxQuantityByBudget, maxQuantity);

                for (int quantity = minQuantity; quantity <= maxQty; quantity++)
                {
                    decimal cost = quantity * flowerCandidate.Flower.FlowerCost;
                    if (cost > roleBudget)
                    {
                        continue;
                    }

                    double harmonyScore = quantity * flowerCandidate.Harmony;

                    if (harmonyScore > bestHarmonyScore)
                    {
                        bestHarmonyScore = harmonyScore;
                        roleComposition = new Dto.FlowerComposition
                        (
                            flowerCandidate.Flower,
                            flowerCandidate.Role,
                            quantity,
                            cost
                        );
                    }
                }
            }

            return roleComposition;
        }
    }
}
