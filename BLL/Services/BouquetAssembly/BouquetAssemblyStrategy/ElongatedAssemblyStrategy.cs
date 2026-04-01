using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;

namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    public class ElongatedAssemblyStrategy : IBouquetAssemblyStrategy
    {
        public AssemblyResult AssembleBouquet(List<ProcessedFlower> processedFlowersForAssembly)
        {
            var layoutCoordinates = new List<FlowerCoordinate>();

            // 1. Створюємо дві окремі черги
            var focalQueue = new Queue<ProcessedFlower>(processedFlowersForAssembly.Where(f => f.Item.Role == "Focal"));
            var mixedQueue = PrepareBalancedSequence(processedFlowersForAssembly.Where(f => f.Item.Role != "Focal").ToList());

            double currentRadiusX = 0;
            double currentRadiusY = 0;

            // 2. ЦЕНТР: Абсолютний якір композиції
            // Беремо першу квітку (бажано фокальну, якщо є) і ставимо в (0,0)
            ProcessedFlower centerFlower = null;

            // Спочатку шукаємо кандидата серед фокальних, якщо їх немає - беремо з міксу
            if (focalQueue.Count > 0)
            {
                centerFlower = focalQueue.Dequeue();
            }
            else if (mixedQueue.Count > 0)
            {
                centerFlower = mixedQueue.Dequeue();
            }

            // Якщо хоч якась квітка є, ставимо її в центр і рахуємо стартові радіуси
            if (centerFlower != null)
            {
                layoutCoordinates.Add(new FlowerCoordinate(centerFlower, 0, 0));

                // Динамічний розрахунок стартового еліпса для БУДЬ-ЯКОЇ квітки в центрі
                // X (ширина) робимо в 1.5 рази більшим за розмір бутона
                // Y (глибина) робимо вужчим, щоб одразу задати форму місяця
                currentRadiusX = centerFlower.Item.HeadSizeCm * 1.5;
                currentRadiusY = centerFlower.Item.HeadSizeCm * 0.5;
            }

            // 3. БУДУЄМО ДУГИ (Хребет + Обгортка)
            // Спочатку вибудовуємо дуги тільки з фокальних квітів (формуємо хребет місяця)
            BuildCrescentLayers(focalQueue, ref currentRadiusX, ref currentRadiusY, layoutCoordinates);

            // Потім продовжуємо нарощувати ці ж самі дуги, але вже міксом інших квітів
            BuildCrescentLayers(mixedQueue, ref currentRadiusX, ref currentRadiusY, layoutCoordinates);

            return new AssemblyResult(true, DateTime.Now, "Crescent/Elongated", Math.Round(currentRadiusX * 2, 1), layoutCoordinates);
        }

        private void BuildCrescentLayers(Queue<ProcessedFlower> queue, ref double currentRadiusX, ref double currentRadiusY, List<FlowerCoordinate> layout)
        {
            while (queue.Count > 0)
            {
                double nextRadius = queue.Peek().Item.HeadSizeCm / 2.0;

                // Рахуємо довжину дуги за поточними currentRadiusX та currentRadiusY
                double arcLength = Math.PI * Math.Sqrt((currentRadiusX * currentRadiusX + currentRadiusY * currentRadiusY) / 2.0);

                // Місткість дуги
                int capacity = (int)Math.Floor(arcLength / (nextRadius * 2.5));
                capacity = Math.Max(1, capacity); // Навіть у найменшу дугу ставимо хоча б 1 квітку
                capacity = Math.Min(capacity, queue.Count);

                var currentArcFlowers = TakeFromQueue(queue, capacity);
                CalculateCrescentLayerPositions(currentArcFlowers, currentRadiusX, currentRadiusY, layout);

                // РОЗШИРЕННЯ ДЛЯ НАСТУПНОГО РЯДУ
                // X росте швидко (тягнеться в боки), Y росте повільно (тримає площину)
                currentRadiusX += (nextRadius * 2.5);
                currentRadiusY += (nextRadius * 1.0);
            }
        }

        // --- РОЗРАХУНОК КООРДИНАТ ДУГИ ---

        private void CalculateCrescentLayerPositions(List<ProcessedFlower> layerFlowers, double ellipseWidthRadius, double ellipseDepthRadius, List<FlowerCoordinate> bouquetLayout)
        {
            int flowerCount = layerFlowers.Count;
            if (flowerCount == 0)
            {
                return;
            }

            if (flowerCount == 1)
            {
                // Якщо в цьому шарі лише одна квітка, ставимо її на вершину дуги (максимальне виступання вперед)
                bouquetLayout.Add(new FlowerCoordinate(layerFlowers[0], 0, Math.Round(ellipseDepthRadius, 3)));
                return;
            }

            // Розподіляємо квіти вздовж півкола (від 0 до 180 градусів, що дорівнює Пі радіан)
            // Ділимо на (flowerCount - 1), щоб перша і остання квітки стояли чітко на кінцях "рогів" місяця
            double radiansBetweenFlowers = Math.PI / (flowerCount - 1);

            for (int i = 0; i < flowerCount; i++)
            {
                double currentAngleRadians = i * radiansBetweenFlowers;

                // X відповідає за розмах букета вліво-вправо (ширину)
                double horizontalPos = Math.Round(ellipseWidthRadius * Math.Cos(currentAngleRadians), 3);

                // Y відповідає за те, наскільки квітка виступає вперед (глибину/вигин)
                double depthPos = Math.Round(ellipseDepthRadius * Math.Sin(currentAngleRadians), 3);

                // Нахил квітки: чим ближче до країв дуги, тим сильніше квітка нахиляється назовні
                double outwardTiltAngle = Math.Abs(Math.Cos(currentAngleRadians)) * 30.0;

                bouquetLayout.Add(new FlowerCoordinate(layerFlowers[i], horizontalPos, depthPos, TiltAngle: outwardTiltAngle));
            }
        }

        private Queue<ProcessedFlower> PrepareBalancedSequence(List<ProcessedFlower> flowers)
        {
            // Реалізація Round-Robin міксера з попереднього кроку
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
