using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowerProcessingStep;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;
namespace BLL.Services.BouquetAssembly.FlowersProcessingStep
{
    public class FlowerProcessingStep : IFlowerProcessingStep
    {
        public AssemblyFlowerItem ProcessFlower(AssemblyFlowerItem flowerItem)
        {
            var flowerItemStemProfile = StemPhysicsProvider.GetProfile(flowerItem.StemKind);
            var actualGripForce = CalculateGripForce(flowerItem, flowerItemStemProfile);
            var flowerItemAfterCutting = Cut(flowerItem, actualGripForce, flowerItemStemProfile);
            var flowerItemAfterStripping = Strip(flowerItemAfterCutting, flowerItemStemProfile);
        }

        public double CalculateGripForce(AssemblyFlowerItem item, StemProfile flowerStemProfile)
        {
            var weightGr = (item.HeadSizeCm * item.StemThicknessMm) + (FlowerProcessingConstants.DefaultSupplierLengthCm * item.StemThicknessMm * flowerStemProfile.WeightFactor)/10;

            double massKg = weightGr / 1000.0;

            // 2. Базова формула сили тертя
            // Сила захвату має бути достатньою, щоб тертя подолало вагу + інерцію
            double requiredForce = (massKg * (GripperConstants.G + GripperConstants.RobotAcceleration)) /
            (flowerStemProfile.Friction * GripperConstants.GripperFingers);

            double finalForce = requiredForce * GripperConstants.SafetyFactor;

            // 3. План Б: Корекція за гнучкістю (Flexibility)
            // Якщо квітка НЕ гнучка (крихка), ми обмежуємо максимальну силу,
            // щоб не зламати стебло, навіть якщо вона важка.
            if (!flowerStemProfile.IsFlexible)
            {
                double maxSafeForce = item.StemThicknessMm * 0.8; // Емпіричне обмеження
                finalForce = Math.Min(finalForce, maxSafeForce);
            }

            return Math.Round(finalForce, 2); // Повертаємо силу в Ньютонах (N)
        }

        public ProcessedFlower Cut(AssemblyFlowerItem flowerItem, double gripForce, StemProfile flowerStemProfile)
        {
            double cuttingResistance = (flowerItem.StemThicknessMm * flowerStemProfile.WeightFactor) * 0.5;

            if (gripForce < cuttingResistance)
            {
                // Дотискаємо захват перед тим, як опустити лезо
                gripForce = cuttingResistance * 1.5;
            }

            // 1. Отримуємо кут з бази/профілю (вже реалізовано в профілі)
            double angle = flowerStemProfile.CutAngle;

            // 2. Логіка підрізання:
            int targetLengthCm = FlowerProcessingConstants.DefaultSupplierLengthCm - FlowerProcessingConstants.CuttingLengthCm;

            // 3. Розрахунок маси ПІСЛЯ підрізання (важливо для фінальної ваги букета)
            double finalWeightGr = (flowerItem.HeadSizeCm * flowerItem.StemThicknessMm) +
                                   (targetLengthCm * flowerItem.StemThicknessMm * flowerStemProfile.WeightFactor) / 10;

            return new ProcessedFlower
            (
                flowerItem,
                gripForce,
                targetLengthCm,
                angle,
                Math.Round(finalWeightGr, 1),
                false
            );
        }

        public ProcessedFlower Strip(ProcessedFlower processedFlower, StemProfile flowerStemProfile)
        {
            // 2. РОЗРАХУНОК ВТРАТИ МАСИ (Фізика процесу)
            // Рахуємо масу ділянки стебла довжиною 25 см:
            double strippedSegmentMassGr = (FlowerProcessingConstants.StrippingLengthCm * processedFlower.Item.StemThicknessMm * flowerStemProfile.WeightFactor) / 10;

            // Маса листя, яке ми зняли стрипером:
            double leavesMassGr = strippedSegmentMassGr * FlowerProcessingConstants.LeafMassReductionFactor;

            // Нова фінальна маса квітки після очищення
            double newWeightGr = processedFlower.FinalWeight - leavesMassGr;

            // 3. ПОВЕРНЕННЯ ОНОВЛЕНОГО СТАНУ
            // Створюємо новий об'єкт (або оновлюємо існуючий) із зафіксованим статусом
            return processedFlower with
            {
                FinalWeight = Math.Round(newWeightGr, 1),
                IsStemCleaned = true,
            };
        }
    }
}
