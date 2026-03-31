using BLL.Services.BouquetAssembly.Descriptors;
using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowerProcessingStep;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;
using BLL.Services.BouquetAssembly.Responses;

namespace BLL.Services.BouquetAssembly
{
    public class BouquetAssembly : IBouquetAssembly
    {
        private readonly IFlowerProcessingStep _flowerProcessingStep;
        public BouquetAssembly(IFlowerProcessingStep flowerProcessingStep)
        {
            _flowerProcessingStep = flowerProcessingStep;
        }
        public AssemblyResult ExecuteAssembly(AssemblyPlanDescriptor plan)
        {
            // ЕТАП 1: Сортування (Фокус -> Другорядні -> Філлери -> Зелень)
            var sortedFlowers = SortFlowersForAssembly(plan.Flowers);

            // ЕТАП 2: Фізична підготовка кожної квітки (Cut, Strip)
            var processedFlowers = new List<ProcessedFlower>();

            foreach (var flower in sortedFlowers)
            {
                // Для кожної одиниці кількості (якщо Quantity > 1)
                for (int i = 0; i < flower.Quantity; i++)
                {
                    // Тут твої методи підрізання та обскубування (імітація)
                    var processedFlower = _flowerProcessingStep.ProcessFlower(flower);
                    processedFlowers.Add(processedFlower);
                }
            }

            // ЕТАП 3: Розрахунок розміщення (Placement/Assembly)
            // Ми передаємо ВЕСЬ список готових квітів у стратегію. 
            // Вона сама розрахує X, Y, Z для кожної квітки.
            var shapeBuilder = BouquetAssemblyFactory.GetStrategy(plan.Shape);
            var assembledBouquet = shapeBuilder.AssembleBouquet(processedFlowers);

            // ЕТАП 4: Формування результату
            return new AssemblyResult
            {
                IsAssembled = true,
                CompletionTime = DateTime.Now,
                FinalWidthCm = layoutResult.FinalWidthCm,
                FinalHeightCm = 40.0, // Наша константа
                AssemblyType = plan.Shape,
                // Сюди можна додати розраховані координати для візуалізації
                FlowerCoordinates = layoutResult.Coordinates
            };
        }

        private IEnumerable<AssemblyFlowerItem> SortFlowersForAssembly(IEnumerable<AssemblyFlowerItem> flowers)
        {
            return flowers.OrderBy(f => f.Role switch
            {
                "Focal" => 1,
                "Semi" => 2,
                "Filler" => 3,
                "Greenery" => 4,
                _ => 5
            });
        }
    }
}
