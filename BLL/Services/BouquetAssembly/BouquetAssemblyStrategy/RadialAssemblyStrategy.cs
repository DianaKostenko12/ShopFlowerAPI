using BLL.Services.BouquetAssembly.Descriptors;
using BLL.Services.BouquetAssembly.Responses;
using BLL.Services.BouquetGeneration.BouquetPlanner;

namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    public class RadialAssemblyStrategy : IBouquetAssemblyStrategy
    {
        public AssemblyResult Assemble(AssemblyPlanDescriptor plan)
        {
            double maxFinalHeight = 0;
            double currentRadius = 0;
            double totalWeight = 0;

            foreach (var item in plan.Flowers)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    double cutLength = ResolveLength(item.Role);

                    totalWeight += item.HeadSizeCm * 1.5 + cutLength * 0.4;

                    if (item.Role == RolesConstants.FocalCategory)
                    {
                        currentRadius += item.HeadSizeCm * 0.05;
                    }
                    else
                    {
                        currentRadius += item.HeadSizeCm * 0.12;
                    }

                    if (cutLength > maxFinalHeight)
                    {
                        maxFinalHeight = cutLength;
                    }
                }
            }

            return new AssemblyResult
            {
                IsAssembled = true,
                CompletionTime = new DateTime(12, 02, 2026),
                FinalHeightCm = maxFinalHeight,
                FinalWidthCm = Math.Round(currentRadius * 2, 1),
                AssemblyType = "Radial"
            };
        }

        private double ResolveLength(string role) => role switch
        {
            RolesConstants.FocalCategory => 45.0,
            RolesConstants.SemiCategory => 42.0,
            RolesConstants.FillerCategory => 38.0,
            RolesConstants.GreeneryCategory => 35.0,
            _ => 40.0
        };
    }
}
