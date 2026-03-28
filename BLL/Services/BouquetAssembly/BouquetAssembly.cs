using BLL.Services.BouquetAssembly.Descriptors;
using BLL.Services.BouquetAssembly.Responses;

namespace BLL.Services.BouquetAssembly
{
    public class BouquetAssembly : IBouquetAssembly
    {
        public AssemblyResult ExecuteAssembly(AssemblyPlanDescriptor plan)
        {
            double accumulatedRadius = 0;
            double maxHeight = 0;

            var layers = plan.Flowers.GroupBy(f => f.Role);

            foreach (var layer in layers)
            {
                var maxHeadInLayer = layer.Max(f => f.HeadSizeCm);
                var totalFlowersInLayer = layer.Sum(f => f.Quantity);

                double requiredCircumference = totalFlowersInLayer * maxHeadInLayer;
                double layerRadius = requiredCircumference / (2 * Math.PI);

                accumulatedRadius += (maxHeadInLayer / 2.0);

                maxHeight = Math.Max(maxHeight, 40.0);
            }

            return new AssemblyResult
            {
                IsAssembled = true,
                CompletionTime = DateTime.Now,
                FinalWidthCm = Math.Round(accumulatedRadius * 2, 1),
                FinalHeightCm = maxHeight,
                AssemblyType = plan.Shape
            };
        }
    }
}
