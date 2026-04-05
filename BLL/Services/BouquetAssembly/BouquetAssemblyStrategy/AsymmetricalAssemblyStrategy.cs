using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;
using DAL.Models;

namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    public class AsymmetricalAssemblyStrategy : IBouquetAssemblyStrategy
    {
        public LayoutResult AssembleBouquet(List<ProcessedFlower> processedFlowersForAssembly)
        {
            var layoutCoordinates = new List<FlowerCoordinate>();

            var greeneryQueue = new Queue<ProcessedFlower>(
                processedFlowersForAssembly.Where(f => f.Item.Role == FlowerRole.Greenery));

            var sortedFlowersByHeadSize = processedFlowersForAssembly
                .Where(f => f.Item.Role != FlowerRole.Greenery)
                .OrderBy(f => f.Item.HeadSizeCm)
                .ToList();

            if (sortedFlowersByHeadSize.Count == 0 && greeneryQueue.Count == 0)
            {
                return new LayoutResult(true, DateTime.Now, "Asymmetrical", 0, layoutCoordinates);
            }

            ProcessedFlower centerFlower;
            if (sortedFlowersByHeadSize.Count > 0)
            {
                centerFlower = sortedFlowersByHeadSize[0];
                sortedFlowersByHeadSize.RemoveAt(0);
            }
            else
            {
                centerFlower = greeneryQueue.Dequeue();
            }

            layoutCoordinates.Add(new FlowerCoordinate(centerFlower, 0, 0));

            var flowerQueue = new Queue<ProcessedFlower>(sortedFlowersByHeadSize);
            int estimatedRings = EstimateRingCount(sortedFlowersByHeadSize);
            int greeneryPerRing = estimatedRings > 0
                ? Math.Max(1, greeneryQueue.Count / estimatedRings)
                : greeneryQueue.Count;

            double currentRadius = centerFlower.Item.HeadSizeCm;
            double cumulativeShiftX = 0;
            int ringIndex = 0;

            while (flowerQueue.Count > 0)
            {
                double nextFlowerRadius = flowerQueue.Peek().Item.HeadSizeCm / 2.0;
                currentRadius += nextFlowerRadius * AsymmetricalAssemblyConstants.AirinessCoefficient;
                cumulativeShiftX += nextFlowerRadius * AsymmetricalAssemblyConstants.AsymmetryShiftFactor;

                int ringCapacity = CalculateRingCapacity(nextFlowerRadius, currentRadius);
                ringCapacity = Math.Min(ringCapacity, flowerQueue.Count);

                var ringFlowers = TakeFromQueue(flowerQueue, ringCapacity);

                int greeneryCount = Math.Min(greeneryPerRing, greeneryQueue.Count);
                var ringGreenery = TakeFromQueue(greeneryQueue, greeneryCount);

                PlaceAsymmetricRing(ringFlowers, ringGreenery, currentRadius, cumulativeShiftX, ringIndex, layoutCoordinates);
                ringIndex++;
            }

            if (greeneryQueue.Count > 0)
            {
                currentRadius += 2.0;
                cumulativeShiftX += 0.5;
                var remainingGreenery = TakeFromQueue(greeneryQueue, greeneryQueue.Count);
                PlaceAsymmetricRing(new List<ProcessedFlower>(), remainingGreenery, currentRadius, cumulativeShiftX, ringIndex, layoutCoordinates);
            }

            double finalWidth = (currentRadius + Math.Abs(cumulativeShiftX)) * 2;
            return new LayoutResult(true, DateTime.Now, "Asymmetrical", Math.Round(finalWidth, 2), layoutCoordinates);
        }

        private void PlaceAsymmetricRing(
            List<ProcessedFlower> ringFlowers,
            List<ProcessedFlower> ringGreenery,
            double radius,
            double shiftX,
            int ringIndex,
            List<FlowerCoordinate> layout)
        {
            var allInRing = InterleaveGreenery(ringFlowers, ringGreenery);
            int totalCount = allInRing.Count;
            if (totalCount == 0)
            {
                return;
            }

            double angleStep = (2 * Math.PI) / totalCount;

            for (int i = 0; i < totalCount; i++)
            {
                double baseAngle = i * angleStep;
                double asymmetricAngle = baseAngle + AsymmetricalAssemblyConstants.AngularDistortionAmplitude * Math.Sin(baseAngle);

                double x = Math.Round(radius * Math.Cos(asymmetricAngle) + shiftX, 3);
                double y = Math.Round(radius * Math.Sin(asymmetricAngle), 3);

                double cosValue = Math.Cos(asymmetricAngle);
                double z = cosValue > 0
                    ? Math.Round(cosValue * AsymmetricalAssemblyConstants.DominantSideMaxHeightCm * (1 + ringIndex * 0.3), 3)
                    : Math.Round(Math.Abs(cosValue) * 1.5, 3);

                double tiltAngle = cosValue > 0
                    ? Math.Abs(cosValue) * AsymmetricalAssemblyConstants.DominantSideTiltMax
                    : Math.Abs(cosValue) * AsymmetricalAssemblyConstants.WeakSideTiltMax;

                layout.Add(new FlowerCoordinate(allInRing[i], x, y, Z: z, TiltAngle: tiltAngle));
            }
        }

        private int CalculateRingCapacity(double flowerRadius, double ringRadius)
        {
            if (flowerRadius >= ringRadius)
            {
                return 1;
            }

            double capacity = Math.PI / Math.Asin(flowerRadius / ringRadius);
            return Math.Max(1, (int)Math.Floor(capacity));
        }

        private int EstimateRingCount(List<ProcessedFlower> flowers)
        {
            return flowers.Count == 0 ? 0 : Math.Max(1, (int)Math.Ceiling(flowers.Count / 6.0));
        }

        private List<ProcessedFlower> InterleaveGreenery(List<ProcessedFlower> flowers, List<ProcessedFlower> greenery)
        {
            if (greenery.Count == 0)
            {
                return new List<ProcessedFlower>(flowers);
            }

            if (flowers.Count == 0)
            {
                return new List<ProcessedFlower>(greenery);
            }

            var result = new List<ProcessedFlower>();
            int flowerIndex = 0, greeneryIndex = 0;
            int total = flowers.Count + greenery.Count;

            for (int i = 0; i < total; i++)
            {
                if (greeneryIndex >= greenery.Count || (flowerIndex < flowers.Count && flowerIndex * greenery.Count <= greeneryIndex * flowers.Count))
                {
                    result.Add(flowers[flowerIndex++]);
                }
                else
                {
                    result.Add(greenery[greeneryIndex++]);
                }
            }

            return result;
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
