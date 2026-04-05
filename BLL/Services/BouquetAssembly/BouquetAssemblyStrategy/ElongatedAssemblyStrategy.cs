using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;
using DAL.Models;

namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    public class ElongatedAssemblyStrategy : IBouquetAssemblyStrategy
    {
        public LayoutResult AssembleBouquet(List<ProcessedFlower> processedFlowersForAssembly)
        {
            var layoutCoordinates = new List<FlowerCoordinate>();

            var focalQueue = new Queue<ProcessedFlower>(processedFlowersForAssembly.Where(f => f.Item.Role == FlowerRole.Focal));
            var mixedQueue = PrepareBalancedSequence(processedFlowersForAssembly.Where(f => f.Item.Role != FlowerRole.Focal).ToList());

            double currentRadiusX = 0;
            double currentRadiusY = 0;

            ProcessedFlower centerFlower = null;

            if (focalQueue.Count > 0)
            {
                centerFlower = focalQueue.Dequeue();
            }
            else if (mixedQueue.Count > 0)
            {
                centerFlower = mixedQueue.Dequeue();
            }

            if (centerFlower != null)
            {
                layoutCoordinates.Add(new FlowerCoordinate(centerFlower, 0, 0));
                currentRadiusX = centerFlower.Item.HeadSizeCm * 1.5;
                currentRadiusY = centerFlower.Item.HeadSizeCm * 0.5;
            }

            BuildCrescentLayers(focalQueue, ref currentRadiusX, ref currentRadiusY, layoutCoordinates);
            BuildCrescentLayers(mixedQueue, ref currentRadiusX, ref currentRadiusY, layoutCoordinates);

            return new LayoutResult(true, DateTime.Now, "Crescent/Elongated", Math.Round(currentRadiusX * 2, 1), layoutCoordinates);
        }

        private void BuildCrescentLayers(Queue<ProcessedFlower> queue, ref double currentRadiusX, ref double currentRadiusY, List<FlowerCoordinate> layout)
        {
            while (queue.Count > 0)
            {
                double nextRadius = queue.Peek().Item.HeadSizeCm / 2.0;
                double arcLength = Math.PI * Math.Sqrt((currentRadiusX * currentRadiusX + currentRadiusY * currentRadiusY) / 2.0);

                int capacity = (int)Math.Floor(arcLength / (nextRadius * 2.5));
                capacity = Math.Max(1, capacity);
                capacity = Math.Min(capacity, queue.Count);

                var currentArcFlowers = TakeFromQueue(queue, capacity);
                CalculateCrescentLayerPositions(currentArcFlowers, currentRadiusX, currentRadiusY, layout);

                currentRadiusX += (nextRadius * 2.5);
                currentRadiusY += (nextRadius * 1.0);
            }
        }

        private void CalculateCrescentLayerPositions(List<ProcessedFlower> layerFlowers, double ellipseWidthRadius, double ellipseDepthRadius, List<FlowerCoordinate> bouquetLayout)
        {
            int flowerCount = layerFlowers.Count;
            if (flowerCount == 0)
            {
                return;
            }

            if (flowerCount == 1)
            {
                bouquetLayout.Add(new FlowerCoordinate(layerFlowers[0], 0, Math.Round(ellipseDepthRadius, 3)));
                return;
            }

            double radiansBetweenFlowers = Math.PI / (flowerCount - 1);

            for (int i = 0; i < flowerCount; i++)
            {
                double currentAngleRadians = i * radiansBetweenFlowers;
                double horizontalPos = Math.Round(ellipseWidthRadius * Math.Cos(currentAngleRadians), 3);
                double depthPos = Math.Round(ellipseDepthRadius * Math.Sin(currentAngleRadians), 3);
                double outwardTiltAngle = Math.Abs(Math.Cos(currentAngleRadians)) * 30.0;

                bouquetLayout.Add(new FlowerCoordinate(layerFlowers[i], horizontalPos, depthPos, TiltAngle: outwardTiltAngle));
            }
        }

        private Queue<ProcessedFlower> PrepareBalancedSequence(List<ProcessedFlower> flowers)
        {
            var grouped = flowers.GroupBy(f => f.Item.Role)
                                 .OrderByDescending(g => g.Count())
                                 .Select(g => new Queue<ProcessedFlower>(g))
                                 .ToList();

            var mixedList = new List<ProcessedFlower>();
            while (grouped.Any(g => g.Count > 0))
            {
                foreach (var group in grouped)
                {
                    if (group.Count > 0)
                    {
                        mixedList.Add(group.Dequeue());
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
