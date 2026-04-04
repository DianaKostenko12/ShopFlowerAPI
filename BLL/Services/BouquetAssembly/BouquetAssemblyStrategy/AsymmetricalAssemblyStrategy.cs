using BLL.Services.BouquetAssembly.DTOs;
using BLL.Services.BouquetAssembly.FlowersProcessingStep.DTOs;

namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    public class AsymmetricalAssemblyStrategy : IBouquetAssemblyStrategy
    {
        public LayoutResult AssembleBouquet(List<ProcessedFlower> processedFlowersForAssembly)
        {
            var layoutCoordinates = new List<FlowerCoordinate>();

            // ============================================================
            // 1. РОЗДІЛЕННЯ: зелень окремо, решта квітів — за розміром голови
            //    Ролі (Focal, Semi, Filler) НЕ впливають на позицію.
            //    Єдине правило: маленькі голови → центр, великі → краї.
            // ============================================================
            var greeneryQueue = new Queue<ProcessedFlower>(
                processedFlowersForAssembly.Where(f => f.Item.Role == "Greenery"));

            var sortedFlowersByHeadSize = processedFlowersForAssembly
                .Where(f => f.Item.Role != "Greenery")
                .OrderBy(f => f.Item.HeadSizeCm)
                .ToList();

            if (sortedFlowersByHeadSize.Count == 0 && greeneryQueue.Count == 0)
            {
                return new LayoutResult(true, DateTime.Now, "Asymmetrical", 0, layoutCoordinates);
            }

            // ============================================================
            // 2. ЦЕНТР: найменша квітка ставиться в абсолютний центр (0, 0, 0)
            // ============================================================
            ProcessedFlower centerFlower;
            if (sortedFlowersByHeadSize.Count > 0)
            {
                centerFlower = sortedFlowersByHeadSize[0];
                sortedFlowersByHeadSize.RemoveAt(0);
            }
            else
            {
                centerFlower = greeneryQueue.Dequeue();
            }

            layoutCoordinates.Add(new FlowerCoordinate(centerFlower, 0, 0));

            // ============================================================
            // 3. ПІДГОТОВКА ЧЕРГИ КВІТІВ І РОЗРАХУНОК ПОРЦІЙ ЗЕЛЕНІ
            //    Оцінюємо кількість кілець, щоб порівну розподілити зелень
            // ============================================================
            var flowerQueue = new Queue<ProcessedFlower>(sortedFlowersByHeadSize);
            int estimatedRings = EstimateRingCount(sortedFlowersByHeadSize);
            int greeneryPerRing = estimatedRings > 0
                ? Math.Max(1, greeneryQueue.Count / estimatedRings)
                : greeneryQueue.Count;

            // ============================================================
            // 4. ПОБУДОВА АСИМЕТРИЧНИХ КІЛЕЦЬ
            //
            //    Кожне кільце:
            //    • Центр зміщений вправо на cumulativeShiftX → створює асиметрію
            //    • Кутовий розподіл спотворений: θ' = θ + 0.3·sin(θ)
            //      → квіти на одній стороні стиснуті, на іншій — розріджені
            //    • Z-висота зростає до домінантної сторони (cos(θ) > 0)
            //    • Квіти йдуть від найменших до найбільших (ascending)
            //      → менші голови лишаються ближче до центру
            //      → більші голови потрапляють на зовнішні (бічні) кільця
            // ============================================================
            double currentRadius = centerFlower.Item.HeadSizeCm;
            double cumulativeShiftX = 0;
            int ringIndex = 0;

            while (flowerQueue.Count > 0)
            {
                double nextFlowerRadius = flowerQueue.Peek().Item.HeadSizeCm / 2.0;

                // Радіус кільця зростає пропорційно до розміру наступної квітки
                currentRadius += nextFlowerRadius * AsymmetricalAssemblyConstants.AirinessCoefficient;

                // Накопичувальний зсув: кожне кільце зміщується далі вправо
                cumulativeShiftX += nextFlowerRadius * AsymmetricalAssemblyConstants.AsymmetryShiftFactor;

                // Місткість кільця: n = π / arcsin(r / R)
                int ringCapacity = CalculateRingCapacity(nextFlowerRadius, currentRadius);
                ringCapacity = Math.Min(ringCapacity, flowerQueue.Count);

                var ringFlowers = TakeFromQueue(flowerQueue, ringCapacity);

                // Забираємо порцію зелені для цього кільця
                int greeneryCount = Math.Min(greeneryPerRing, greeneryQueue.Count);
                var ringGreenery = TakeFromQueue(greeneryQueue, greeneryCount);

                PlaceAsymmetricRing(ringFlowers, ringGreenery, currentRadius, cumulativeShiftX, ringIndex, layoutCoordinates);
                ringIndex++;
            }

            // ============================================================
            // 5. ЗАЛИШОК ЗЕЛЕНІ розкидаємо додатковим зовнішнім кільцем
            // ============================================================
            if (greeneryQueue.Count > 0)
            {
                currentRadius += 2.0;
                cumulativeShiftX += 0.5;
                var remainingGreenery = TakeFromQueue(greeneryQueue, greeneryQueue.Count);
                PlaceAsymmetricRing(new List<ProcessedFlower>(), remainingGreenery, currentRadius, cumulativeShiftX, ringIndex, layoutCoordinates);
            }

            double finalWidth = (currentRadius + Math.Abs(cumulativeShiftX)) * 2;
            return new LayoutResult(true, DateTime.Now, "Asymmetrical", Math.Round(finalWidth, 2), layoutCoordinates);
        }

        /// <summary>
        /// Розміщує квіти і зелень в одному асиметричному кільці.
        /// Центр кільця зсунутий на shiftX відносно початку координат.
        /// Кутовий розподіл нелінійний для створення візуальної нерівномірності.
        /// </summary>
        private void PlaceAsymmetricRing(
            List<ProcessedFlower> ringFlowers,
            List<ProcessedFlower> ringGreenery,
            double radius,
            double shiftX,
            int ringIndex,
            List<FlowerCoordinate> layout)
        {
            // Вплітаємо зелень між квітами рівномірно
            var allInRing = InterleaveGreenery(ringFlowers, ringGreenery);
            int totalCount = allInRing.Count;
            if (totalCount == 0)
            {
                return;
            }

            double angleStep = (2 * Math.PI) / totalCount;

            for (int i = 0; i < totalCount; i++)
            {
                // ──── Кутовий розподіл ────
                // Базовий рівномірний кут
                double baseAngle = i * angleStep;

                // Нелінійне спотворення: θ' = θ + A·sin(θ)
                // sin(θ) > 0 (верхня півплощина) → кути розтягуються (квіти рідше)
                // sin(θ) < 0 (нижня півплощина) → кути стискаються (квіти щільніше)
                // Результат: одна сторона букета виглядає "пишнішою" за іншу
                double asymmetricAngle = baseAngle + AsymmetricalAssemblyConstants.AngularDistortionAmplitude * Math.Sin(baseAngle);

                // ──── Декартові координати ────
                // X зсунутий на shiftX → кільце не концентричне, а ексцентричне
                double x = Math.Round(radius * Math.Cos(asymmetricAngle) + shiftX, 3);
                double y = Math.Round(radius * Math.Sin(asymmetricAngle), 3);

                // ──── Висота (Z) — вертикальна асиметрія ────
                // Домінантна сторона (cos > 0, тобто там де зсув) піднімається вище
                // Це створює характерний для асиметричного букета "гребінь" з одного боку
                double cosValue = Math.Cos(asymmetricAngle);
                double z = 0;
                if (cosValue > 0)
                {
                    // Домінантна сторона: висота зростає з кожним кільцем
                    z = Math.Round(cosValue * AsymmetricalAssemblyConstants.DominantSideMaxHeightCm * (1 + ringIndex * 0.3), 3);
                }
                else
                {
                    // Слабка сторона: мінімальна висота
                    z = Math.Round(Math.Abs(cosValue) * 1.5, 3);
                }

                // ──── Нахил квітки (TiltAngle) ────
                // На домінантній стороні квітки нахилені сильніше назовні (до 35°)
                // На слабкій стороні — мінімальний нахил (до 15°)
                double tiltAngle;
                if (cosValue > 0)
                {
                    tiltAngle = Math.Abs(cosValue) * AsymmetricalAssemblyConstants.DominantSideTiltMax;
                }
                else
                {
                    tiltAngle = Math.Abs(cosValue) * AsymmetricalAssemblyConstants.WeakSideTiltMax;
                }

                layout.Add(new FlowerCoordinate(allInRing[i], x, y, Z: z, TiltAngle: tiltAngle));
            }
        }

        /// <summary>
        /// Місткість кільця за формулою: n = π / arcsin(r / R),
        /// де r — радіус бутона квітки, R — радіус кільця.
        /// </summary>
        private int CalculateRingCapacity(double flowerRadius, double ringRadius)
        {
            if (flowerRadius >= ringRadius)
            {
                return 1;
            }

            double capacity = Math.PI / Math.Asin(flowerRadius / ringRadius);
            return Math.Max(1, (int)Math.Floor(capacity));
        }

        /// <summary>
        /// Оцінка кількості кілець на основі загальної кількості квітів.
        /// Потрібна для рівномірного розподілу зелені між кільцями.
        /// </summary>
        private int EstimateRingCount(List<ProcessedFlower> flowers)
        {
            return flowers.Count == 0 ? 0 : Math.Max(1, (int)Math.Ceiling(flowers.Count / 6.0));
        }

        /// <summary>
        /// Рівномірно вплітає зелень між квітами в кільці.
        /// Використовує алгоритм Брезенхема: розподіляє M елементів зелені
        /// серед N квітів з мінімальним відхиленням від ідеального інтервалу.
        /// </summary>
        private List<ProcessedFlower> InterleaveGreenery(List<ProcessedFlower> flowers, List<ProcessedFlower> greenery)
        {
            if (greenery.Count == 0)
            {
                return new List<ProcessedFlower>(flowers);
            }

            if (flowers.Count == 0)
            {
                return new List<ProcessedFlower>(greenery);
            }

            var result = new List<ProcessedFlower>();
            int flowerIndex = 0, greeneryIndex = 0;
            int total = flowers.Count + greenery.Count;

            // Алгоритм Брезенхема для рівномірного розподілу двох послідовностей
            // Гарантує що зелень розподілена максимально рівномірно між квітами
            for (int i = 0; i < total; i++)
            {
                // Порівнюємо пропорції: якщо квітів "відстає" від ідеального темпу — додаємо квітку
                if (greeneryIndex >= greenery.Count || (flowerIndex < flowers.Count && flowerIndex * greenery.Count <= greeneryIndex * flowers.Count))
                {
                    result.Add(flowers[flowerIndex++]);
                }
                else
                {
                    result.Add(greenery[greeneryIndex++]);
                }
            }

            return result;
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
