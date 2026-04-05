using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;
using DAL.Models;

namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    public class RadialAssemblyStrategy : IBouquetAssemblyStrategy
    {
        public LayoutResult AssembleBouquet(List<ProcessedFlower> processedFlowersForAssembly)
        {
            var layoutCoordinates = new List<FlowerCoordinate>();

            var focals = processedFlowersForAssembly.Where(f => f.Item.Role == FlowerRole.Focal).ToList();
            var otherRoles = processedFlowersForAssembly.Where(f => f.Item.Role != FlowerRole.Focal).ToList();

            double currentRadius = 0;

            if (focals.Any())
            {
                layoutCoordinates.Add(new FlowerCoordinate(focals[0], 0, 0));

                if (focals.Count > 1)
                {
                    currentRadius = focals[0].Item.HeadSizeCm;
                    CalculateRingCoordinates(focals.Skip(1).ToList(), currentRadius, layoutCoordinates);
                }
            }

            var balancedFlowerQueue = PrepareBalancedSequence(otherRoles);

            while (balancedFlowerQueue.Count > 0)
            {
                double nextFlowerRadius = balancedFlowerQueue.Peek().Item.HeadSizeCm / 2.0;
                currentRadius += (nextFlowerRadius * 1.5);

                double maxCapacityDouble = Math.PI / Math.Asin(nextFlowerRadius / currentRadius);
                int capacity = (int)Math.Floor(maxCapacityDouble);
                capacity = Math.Min(capacity, balancedFlowerQueue.Count);

                var currentRingFlowers = TakeFromQueue(balancedFlowerQueue, capacity);
                CalculateRingCoordinates(currentRingFlowers, currentRadius, layoutCoordinates);
            }

            return new LayoutResult(true, DateTime.Now, "Radial Exact Math", Math.Round(currentRadius * 2, 2), layoutCoordinates);
        }

        private void CalculateRingCoordinates(List<ProcessedFlower> flowersToBuildRing, double radius, List<FlowerCoordinate> layout)
        {
            int flowersCount = flowersToBuildRing.Count;
            if (flowersCount == 0)
            {
                return;
            }

            double angleStep = (2 * Math.PI) / flowersCount;

            for (int i = 0; i < flowersCount; i++)
            {
                double angle = i * angleStep;
                double x = Math.Round(radius * Math.Cos(angle), 3);
                double y = Math.Round(radius * Math.Sin(angle), 3);

                layout.Add(new FlowerCoordinate(flowersToBuildRing[i], x, y));
            }
        }

        private Queue<ProcessedFlower> PrepareBalancedSequence(List<ProcessedFlower> flowersWithOtherRoles)
        {
            var flowerQueuesByRole = flowersWithOtherRoles.GroupBy(f => f.Item.Role)
                                    .OrderByDescending(g => g.Count())
                                    .Select(g => new Queue<ProcessedFlower>(g))
                                    .ToList();

            var mixedList = new List<ProcessedFlower>();

            while (flowerQueuesByRole.Any(roleQueue => roleQueue.Count > 0))
            {
                foreach (var roleQueue in flowerQueuesByRole)
                {
                    if (roleQueue.Count > 0)
                    {
                        mixedList.Add(roleQueue.Dequeue());
                    }
                }
            }

            return new Queue<ProcessedFlower>(mixedList);
        }

        private List<ProcessedFlower> TakeFromQueue(Queue<ProcessedFlower> queue, int count)
        {
            var result = new List<ProcessedFlower>();
            for (int i = 0; i < count && queue.Count > 0; i++)
            {
                result.Add(queue.Dequeue());
            }
            return result;
        }
    }
}
