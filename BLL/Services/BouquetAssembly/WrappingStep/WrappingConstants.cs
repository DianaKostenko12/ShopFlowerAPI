namespace BLL.Services.BouquetAssembly.WrappingStep
{
    /// <summary>
    /// Константи для етапу обгортання букета папером та фіксації стрічкою.
    /// </summary>
    public static class WrappingConstants
    {
        /// <summary>
        /// Ширина одного нарізаного клаптика обгортального паперу (см).
        /// Визначає яку дугу периметра букета покриває один клаптик.
        /// </summary>
        public const double PaperPatchWidthCm = 25.0;

        /// <summary>
        /// Висота одного клаптика обгортального паперу (см).
        /// Покриває стебла від точки в'язки до нижнього краю бутонів.
        /// </summary>
        public const double PaperPatchHeightCm = 35.0;

        /// <summary>
        /// Коефіцієнт перекриття між сусідніми клаптиками (частка від ширини).
        /// Клаптики накладаються один на одний для щільності обгортки.
        /// Ефективна ширина = PaperPatchWidthCm × (1 - OverlapFactor).
        /// </summary>
        public const double OverlapFactor = 0.2;

        /// <summary>
        /// Довжина стрічки для одного оберту навколо букета (см).
        /// Розраховується динамічно, це мінімальний запас на вузол.
        /// </summary>
        public const double RibbonKnotReserveCm = 15.0;
    }
}
