using BLL.Services.BouquetAssembly.WrappingStep.DTOs;

namespace BLL.Services.BouquetAssembly.WrappingStep
{
    public class BouquetWrappingStep : IBouquetWrappingStep
    {
        /// <summary>
        /// Обгортає зібраний букет клаптиками обгортального паперу та фіксує стрічкою.
        ///
        /// Алгоритм:
        /// 1. Розраховуємо периметр букета: P = π × W (ширина = діаметр)
        /// 2. Ефективна ширина клаптика з урахуванням перекриття:
        ///    effectiveWidth = patchWidth × (1 − overlapFactor)
        /// 3. Кількість клаптиків: N = ⌈P / effectiveWidth⌉
        /// 4. Перша стрічка — фіксує стебла перед обгортанням
        /// 5. Для кожного клаптика: прикладаємо папір → фіксуємо стрічкою
        /// 6. Загальна довжина стрічки: (N + 1) × (P + запас на вузол)
        /// </summary>
        public WrappingResult WrapBouquet(int wrappingPaperId, double bouquetWidthCm)
        {
            var actions = new List<WrappingAction>();
            int stepNumber = 1;

            // ──── Периметр букета ────
            // Букет у перерізі наближено до кола, де ширина = діаметр
            double bouquetPerimeter = Math.PI * bouquetWidthCm;

            // ──── Кількість клаптиків паперу ────
            // Кожен клаптик перекриває попередній на 20% ширини для щільності
            double effectivePatchWidth = WrappingConstants.PaperPatchWidthCm * (1 - WrappingConstants.OverlapFactor);
            int patchesNeeded = (int)Math.Ceiling(bouquetPerimeter / effectivePatchWidth);

            // Мінімум 2 клаптики навіть для маленьких букетів
            patchesNeeded = Math.Max(2, patchesNeeded);

            // ──── КРОК 1: Початкова фіксація стрічкою ────
            // Обв'язуємо стебла один раз перед обгортанням, щоб букет тримав форму
            actions.Add(new WrappingAction(
                stepNumber++,
                "RibbonTie",
                "Початкова фіксація стебел стрічкою перед обгортанням"
            ));

            int ribbonTiesCount = 1;

            // ──── КРОК 2: Послідовне обгортання клаптиками ────
            // Кожен клаптик прикладається до букета, потім фіксується стрічкою
            for (int i = 0; i < patchesNeeded; i++)
            {
                // Прикладаємо клаптик паперу
                double startAngleDegrees = Math.Round(i * (360.0 / patchesNeeded), 1);
                actions.Add(new WrappingAction(
                    stepNumber++,
                    "PaperPatch",
                    $"Клаптик #{i + 1}: прикладаємо на позицію {startAngleDegrees}° периметра " +
                    $"(ширина {WrappingConstants.PaperPatchWidthCm} см, висота {WrappingConstants.PaperPatchHeightCm} см)"
                ));

                // Фіксуємо стрічкою поверх щойно прикладеного клаптика
                actions.Add(new WrappingAction(
                    stepNumber++,
                    "RibbonTie",
                    $"Фіксація стрічкою після клаптика #{i + 1}"
                ));

                ribbonTiesCount++;
            }

            // ──── Загальна довжина стрічки ────
            // Кожна обв'язка = один оберт навколо букета + запас на вузол
            double ribbonPerTie = bouquetPerimeter + WrappingConstants.RibbonKnotReserveCm;
            double totalRibbonLength = Math.Round(ribbonTiesCount * ribbonPerTie, 2);

            return new WrappingResult(
                wrappingPaperId,
                patchesNeeded,
                ribbonTiesCount,
                totalRibbonLength,
                Math.Round(bouquetPerimeter, 2),
                actions
            );
        }
    }
}
