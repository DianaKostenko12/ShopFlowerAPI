using BLL.Services.BouquetAssembly.BouquetAssemblyStrategy;

namespace BLL.Services.BouquetAssembly
{
    public static class BouquetAssemblyFactory
    {
        public static IBouquetAssemblyStrategy GetStrategy(string shape)
        {
            return shape?.ToLowerInvariant() switch
            {
                "кругла" => new RoundAssemblyStrategy(),
                "подовжена" => new ElongatedAssemblyStrategy(),
                "горизонтальна" => new HorizontalAssemblyStrategy(),
                "асиметрична" => new AsymmetricalAssemblyStrategy(),
                _ => new RoundAssemblyStrategy() // Дефолт
            };
        }
    }
}
