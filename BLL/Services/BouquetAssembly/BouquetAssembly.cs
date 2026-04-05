using BLL.Services.BouquetAssembly.Descriptors;
using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowerProcessingStep;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;
using BLL.Services.BouquetAssembly.WrappingStep;
using DAL.Models;
using AssemblyResult = BLL.Services.BouquetAssembly.DTOs.AssemblyResult;

namespace BLL.Services.BouquetAssembly
{
    public class BouquetAssembly : IBouquetAssembly
    {
        private readonly IFlowerProcessingStep _flowerProcessingStep;
        private readonly IBouquetWrappingStep _bouquetWrappingStep;

        public BouquetAssembly(IFlowerProcessingStep flowerProcessingStep, IBouquetWrappingStep bouquetWrappingStep)
        {
            _flowerProcessingStep = flowerProcessingStep;
            _bouquetWrappingStep = bouquetWrappingStep;
        }
        public AssemblyResult ExecuteAssembly(AssemblyPlanDescriptor plan)
        {
            var sortedFlowers = SortFlowersForAssembly(plan.Flowers);

            var processedFlowers = new List<ProcessedFlower>();

            foreach (var flower in sortedFlowers)
            {
                for (int i = 0; i < flower.Quantity; i++)
                {
                    var processedFlower = _flowerProcessingStep.ProcessFlower(flower);
                    processedFlowers.Add(processedFlower);
                }
            }

            var shapeBuilder = BouquetAssemblyFactory.GetStrategy(plan.Shape);
            var assembledBouquet = shapeBuilder.AssembleBouquet(processedFlowers);

            var wrappingResult = _bouquetWrappingStep.WrapBouquet(plan.WrappingPaperId, assembledBouquet.FinalWidthCm);

            return new AssemblyResult(
                assembledBouquet.IsAssembled,
                assembledBouquet.CompletionTime,
                plan.Shape,
                assembledBouquet.FinalWidthCm,
                assembledBouquet.Coordinates,
                wrappingResult
            );
        }

        private IEnumerable<AssemblyFlowerItem> SortFlowersForAssembly(IEnumerable<AssemblyFlowerItem> flowers)
        {
            return flowers.OrderBy(f => f.Role switch
            {
                FlowerRole.Focal => 1,
                FlowerRole.Semi => 2,
                FlowerRole.Filler => 3,
                FlowerRole.Greenery => 4,
                _ => 5
            });
        }
    }
}
