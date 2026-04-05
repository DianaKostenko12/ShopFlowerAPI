using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.OpenAi.Dto;
using DAL.Models;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition
{
    public sealed class FlowerCompositionBuilder : IFlowerCompositionBuilder
    {
        public List<Dto.FlowerComposition> BuildFlowersComposition(List<FlowerWithRole> flowersWithRole, GptStyleRecommendation aiStyleRecommendation, decimal totalBudget)
        {
            List<Dto.FlowerComposition> completedСomposition = new();

            var roleBudgetProportions = new Dictionary<FlowerRole, decimal>
            {
                { FlowerRole.Focal, 0.4m },
                { FlowerRole.Semi, 0.3m },
                { FlowerRole.Filler, 0.2m },
                { FlowerRole.Greenery, 0.1m }
            };

            foreach (var role in roleBudgetProportions.Keys)
            {
                decimal roleBudget = totalBudget * roleBudgetProportions[role];

                var (minQuantity, maxQuantity) = role switch
                {
                    FlowerRole.Focal => (aiStyleRecommendation.Roles.Focal.Min, aiStyleRecommendation.Roles.Focal.Max),
                    FlowerRole.Semi => (aiStyleRecommendation.Roles.Semi.Min, aiStyleRecommendation.Roles.Semi.Max),
                    FlowerRole.Filler => (aiStyleRecommendation.Roles.Filler.Min, aiStyleRecommendation.Roles.Filler.Max),
                    FlowerRole.Greenery => (aiStyleRecommendation.Roles.Greenery.Min, aiStyleRecommendation.Roles.Greenery.Max),
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

                if (role == FlowerRole.Focal)
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
            var roleComposition = new Dto.FlowerComposition(null, default, 0, 0);

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
