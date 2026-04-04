namespace BLL.Services.BouquetAssembly.BouquetAssemblyStrategy
{
    /// <summary>
    /// Константи для алгоритму складання асиметричного букета.
    /// </summary>
    public static class AsymmetricalAssemblyConstants
    {
        /// <summary>
        /// Зсув центру кожного кільця по X для створення асиметрії (частка від радіуса квітки).
        /// Чим більше значення — тим сильніше композиція зміщена в бік.
        /// </summary>
        public const double AsymmetryShiftFactor = 0.35;

        /// <summary>
        /// Амплітуда нелінійного спотворення кутового розподілу.
        /// Формула: θ' = θ + A·sin(θ) стискає квіти з одного боку і розтягує з іншого.
        /// </summary>
        public const double AngularDistortionAmplitude = 0.3;

        /// <summary>
        /// Коефіцієнт повітряності між квітами в кільці.
        /// Більше значення — більша відстань між бутонами.
        /// </summary>
        public const double AirinessCoefficient = 1.6;

        /// <summary>
        /// Максимальний нахил квітки назовні на домінантній стороні (градуси).
        /// </summary>
        public const double DominantSideTiltMax = 35.0;

        /// <summary>
        /// Максимальний нахил квітки назовні на слабкій стороні (градуси).
        /// </summary>
        public const double WeakSideTiltMax = 15.0;

        /// <summary>
        /// Максимальна висота (Z) для квітів на домінантній стороні (см).
        /// Створює характерний для асиметричного букета вертикальний "гребінь".
        /// </summary>
        public const double DominantSideMaxHeightCm = 6.0;
    }
}
