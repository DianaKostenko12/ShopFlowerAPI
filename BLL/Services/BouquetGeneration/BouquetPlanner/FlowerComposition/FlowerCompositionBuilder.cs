using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.OpenAi.Dto;
using DAL.Models;

namespace BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition
{
    public sealed class FlowerCompositionBuilder : IFlowerCompositionBuilder
    {
        private static readonly IReadOnlyDictionary<FlowerRole, decimal> RoleBudgetProportions = new Dictionary<FlowerRole, decimal>
        {
            { FlowerRole.Focal, 0.4m },
            { FlowerRole.Semi, 0.3m },
            { FlowerRole.Filler, 0.2m },
            { FlowerRole.Greenery, 0.1m }
        };

        private static readonly IReadOnlyDictionary<FlowerRole, decimal> RoleQuantityPriorities = new Dictionary<FlowerRole, decimal>
        {
            { FlowerRole.Focal, 0.55m },
            { FlowerRole.Semi, 0.95m },
            { FlowerRole.Filler, 1.25m },
            { FlowerRole.Greenery, 0.75m }
        };

        public List<Dto.FlowerComposition> BuildFlowersComposition(List<FlowerWithRole> flowersWithRole, GptStyleRecommendation aiStyleRecommendation, decimal totalBudget)
        {
            List<Dto.FlowerComposition> completedСomposition = new();

            foreach (var role in RoleBudgetProportions.Keys)
            {
                var flowerCandidates = flowersWithRole
                    .Where(f => f.Role == role)
                    .OrderByDescending(f => (decimal)f.Harmony / f.Flower.FlowerCost)
                    .ToList();

                if (!flowerCandidates.Any())
                {
                    continue;
                }

                decimal roleBudget = totalBudget * RoleBudgetProportions[role];
                int minQuantity = GetRoleMinQuantity(aiStyleRecommendation, role);
                int maxQuantity = GetBudgetDrivenMaxQuantity(flowerCandidates, roleBudget, minQuantity);

                if (maxQuantity < minQuantity)
                {
                    continue;
                }

                if (role == FlowerRole.Focal)
                {
                    var focalRoleComposition = ComposeFlowersForFocalRole(flowerCandidates, roleBudget, maxQuantity, minQuantity);
                    if (focalRoleComposition.flower != null && focalRoleComposition.Quantity > 0)
                    {
                        completedСomposition.Add(focalRoleComposition);
                    }
                }
                else
                {
                    var roleComposition = ComposeFlowersForRole(flowerCandidates, roleBudget, maxQuantity, minQuantity);
                    completedСomposition.AddRange(roleComposition);
                }
            }

            FillRemainingBudget(completedСomposition, flowersWithRole, totalBudget);

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
                .OrderByDescending(s => s.Key.Quantity)
                .ThenByDescending(s => s.Key.Cost)
                .ThenByDescending(s => s.Value.Harmony)
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
            var bestQuantity = 0;
            var bestCost = 0m;

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

                    if (quantity > bestQuantity
                        || (quantity == bestQuantity && cost > bestCost)
                        || (quantity == bestQuantity && cost == bestCost && harmonyScore > bestHarmonyScore))
                    {
                        bestQuantity = quantity;
                        bestCost = cost;
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

        private static int GetRoleMinQuantity(GptStyleRecommendation aiStyleRecommendation, FlowerRole role)
        {
            return role switch
            {
                FlowerRole.Focal => aiStyleRecommendation.Roles.Focal.Min,
                FlowerRole.Semi => aiStyleRecommendation.Roles.Semi.Min,
                FlowerRole.Filler => aiStyleRecommendation.Roles.Filler.Min,
                FlowerRole.Greenery => aiStyleRecommendation.Roles.Greenery.Min,
                _ => throw new ArgumentOutOfRangeException(nameof(role))
            };
        }

        private static int GetBudgetDrivenMaxQuantity(List<FlowerWithRole> flowers, decimal roleBudget, int minQuantity)
        {
            var cheapestFlowerCost = flowers.Min(f => f.Flower.FlowerCost);
            if (cheapestFlowerCost <= 0)
            {
                return minQuantity;
            }

            return Math.Max(minQuantity, (int)(roleBudget / cheapestFlowerCost));
        }

        private static void FillRemainingBudget(
            List<Dto.FlowerComposition> completedComposition,
            List<FlowerWithRole> flowersWithRole,
            decimal totalBudget)
        {
            if (completedComposition.Count == 0)
            {
                return;
            }

            decimal spentBudget = completedComposition.Sum(item => item.UnitPrice);
            decimal remainingBudget = totalBudget - spentBudget;

            while (remainingBudget > 0)
            {
                var currentRoleQuantities = completedComposition
                    .GroupBy(item => item.Role)
                    .ToDictionary(group => group.Key, group => group.Sum(item => item.Quantity));

                var nextFlower = flowersWithRole
                    .Where(candidate =>
                        candidate.Flower.FlowerCost <= remainingBudget)
                    .OrderByDescending(candidate => GetBudgetFillPriority(candidate, currentRoleQuantities))
                    .ThenBy(candidate => candidate.Flower.FlowerCost)
                    .FirstOrDefault();

                if (nextFlower == null)
                {
                    break;
                }

                var existingItemIndex = completedComposition.FindIndex(item => item.flower.FlowerId == nextFlower.Flower.FlowerId);
                if (existingItemIndex >= 0)
                {
                    var existingItem = completedComposition[existingItemIndex];
                    completedComposition[existingItemIndex] = existingItem with
                    {
                        Quantity = existingItem.Quantity + 1,
                        UnitPrice = existingItem.UnitPrice + nextFlower.Flower.FlowerCost
                    };
                }
                else
                {
                    completedComposition.Add(new Dto.FlowerComposition(
                        nextFlower.Flower,
                        nextFlower.Role,
                        1,
                        nextFlower.Flower.FlowerCost));
                }

                remainingBudget -= nextFlower.Flower.FlowerCost;
            }
        }

        private static decimal GetBudgetFillPriority(FlowerWithRole candidate, IReadOnlyDictionary<FlowerRole, int> currentRoleQuantities)
        {
            var currentRoleQuantity = currentRoleQuantities.GetValueOrDefault(candidate.Role);
            var quantityNeedFactor = RoleQuantityPriorities.GetValueOrDefault(candidate.Role, 1m) / (currentRoleQuantity + 1m);

            return ((decimal)candidate.Harmony / candidate.Flower.FlowerCost) * quantityNeedFactor;
        }
    }
}
