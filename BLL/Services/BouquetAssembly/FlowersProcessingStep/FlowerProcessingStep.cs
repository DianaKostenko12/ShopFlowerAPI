using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowerProcessingStep;
using static System.Net.Mime.MediaTypeNames;
namespace BLL.Services.BouquetAssembly.FlowersProcessingStep
{
    public class FlowerProcessingStep : IFlowerProcessingStep
    {
        public AssemblyFlowerItem ProcessFlower(AssemblyFlowerItem flowerItem)
        {
            
        }

        public double CalculateGripForce(AssemblyFlowerItem item)
        {
            var flowerStemProfile = StemPhysicsProvider.GetProfile(item.StemKind);
            var weight = (item.HeadSizeCm * item.StemThicknessMm) + (GripperConstants.)

            // 2. Базова формула сили тертя
            // Сила захвату має бути достатньою, щоб тертя подолало вагу + інерцію
            double requiredForce = (massKg * (G + RobotAcceleration)) /
            (item.FrictionCoeff * GripperFingers);

            double finalForce = requiredForce * SafetyFactor;

            // 3. План Б: Корекція за гнучкістю (Flexibility)
            // Якщо квітка НЕ гнучка (крихка), ми обмежуємо максимальну силу,
            // щоб не зламати стебло, навіть якщо вона важка.
            if (!item.Flexibility)
            {
                double maxSafeForce = item.StemThicknessMm * 0.8; // Емпіричне обмеження
                finalForce = Math.Min(finalForce, maxSafeForce);
            }

            return Math.Round(finalForce, 2); // Повертаємо силу в Ньютонах (N)
        }
    }
}
