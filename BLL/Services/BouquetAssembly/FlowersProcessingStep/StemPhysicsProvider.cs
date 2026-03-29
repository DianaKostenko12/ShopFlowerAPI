using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;
using DAL.Models.Flowers;

namespace BLL.Services.BouquetAssembly.FlowersProcessingStep
{
    public static class StemPhysicsProvider
    {
        public static StemProfile GetProfile(StemType type) => type switch
        {
            StemType.Soft => new(CutAngle: 90, WeightFactor: 0.8, Friction: 0.2, IsFlexible: true),
            StemType.Standard => new(CutAngle: 45, WeightFactor: 1.0, Friction: 0.4, IsFlexible: true),
            StemType.Woody => new(CutAngle: 45, WeightFactor: 1.5, Friction: 0.6, IsFlexible: false),
            StemType.Succulent => new(CutAngle: 90, WeightFactor: 1.2, Friction: 0.3, IsFlexible: false),
            _ => new(45, 1.0, 0.4, true)
        };
    }
}
