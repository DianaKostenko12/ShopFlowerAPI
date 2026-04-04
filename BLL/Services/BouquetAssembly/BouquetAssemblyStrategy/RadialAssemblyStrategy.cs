using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;

namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    public class RadialAssemblyStrategy : IBouquetAssemblyStrategy
    {
        public LayoutResult AssembleBouquet(List<ProcessedFlower> processedFlowersForAssembly)
        {
            var layoutCoordinates = new List<FlowerCoordinate>();

            // 1. Розділяємо на Центр (Focal) та Периферію (Інші)
            var focals = processedFlowersForAssembly.Where(f => f.Item.Role == "Focal").ToList();
            var otherRoles = processedFlowersForAssembly.Where(f => f.Item.Role != "Focal").ToList();

            double currentRadius = 0;

            // 2. ЦЕНТР (Focal Core)
            if (focals.Any())
            {
                // Ставимо першу фокальну в абсолютний центр
                layoutCoordinates.Add(new FlowerCoordinate(focals[0], 0, 0));

                if (focals.Count > 1)
                {
                    // Розраховуємо радіус першого кільця так, щоб бутони торкалися
                    currentRadius = focals[0].Item.HeadSizeCm;
                    CalculateRingCoordinates(focals.Skip(1).ToList(), currentRadius, layoutCoordinates);
                }
            }

            // 3. ПОБУДОВА КІЛЕЦЬ ДЛЯ ІНШИХ КВІТІВ
            // Міксуємо залишок "розумним" алгоритмом
            var balancedFlowerQueue = PrepareBalancedSequence(otherRoles);

            while (balancedFlowerQueue.Count > 0)
            {
                // Динамічний крок радіуса: беремо розмір наступної квітки в черзі
                double nextFlowerRadius = balancedFlowerQueue.Peek().Item.HeadSizeCm / 2.0;

                // Збільшуємо загальний радіус букета, щоб квіти не налізли на попереднє кільце
                currentRadius += (nextFlowerRadius * 1.5); // 1.5 - коефіцієнт легкої "повітряності" букета

                // РОЗУМНА МАТЕМАТИКА: Скільки квітів реально влізе в це кільце?
                // n = PI / arcsin(r / R)
                double maxCapacityDouble = Math.PI / Math.Asin(nextFlowerRadius / currentRadius);
                int capacity = (int)Math.Floor(maxCapacityDouble);

                // Якщо до кінця черги залишилося менше квітів, ніж місткість кільця, беремо залишок
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

            // Точний кут між центрами квітів у цьому кільці
            double angleStep = (2 * Math.PI) / flowersCount;

            for (int i = 0; i < flowersCount; i++)
            {
                double angle = i * angleStep;

                // Декартові координати з високою точністю
                double x = Math.Round(radius * Math.Cos(angle), 3);
                double y = Math.Round(radius * Math.Sin(angle), 3);

                layout.Add(new FlowerCoordinate(flowersToBuildRing[i], x, y));
            }
        }

        private Queue<ProcessedFlower> PrepareBalancedSequence(List<ProcessedFlower> flowersWithOtherRoles)
        {
            // Групуємо квіти за ролями і рахуємо їх кількість
            var flowerQueuesByRole = flowersWithOtherRoles.GroupBy(f => f.Item.Role)
                                    .OrderByDescending(g => g.Count())
                                    .Select(g => new Queue<ProcessedFlower>(g))
                                    .ToList();

            var mixedList = new List<ProcessedFlower>();

        // Алгоритм рівномірного вплетення (Round-Robin)
        // Він бере по одній квітці з найбільших груп, створюючи ідеальний паттерн
            while (flowerQueuesByRole.Any(roleQueue => roleQueue.Count > 0))
            {
                foreach (var roleQueue in flowerQueuesByRole)
                {
                    // Якщо в цій конкретній черзі ще залишилися квіти
                    if (roleQueue.Count > 0)
                    {
                        // Беремо верхню квітку і додаємо в наш фінальний змішаний список
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
