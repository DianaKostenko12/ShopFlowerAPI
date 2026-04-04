using BLL.Services.BouquetAssembly.BouquetAssemblyStrategy;

namespace BLL.Services.BouquetAssembly
{
    public static class BouquetAssemblyFactory
    {
        public static IBouquetAssemblyStrategy GetStrategy(string shape)
        {
            return shape?.ToLowerInvariant() switch
            {
                "кругла" => new RadialAssemblyStrategy(),
                "подовжена" => new ElongatedAssemblyStrategy(),
                "асиметрична" => new AsymmetricalAssemblyStrategy(),
                _ => new RadialAssemblyStrategy()
            };
        }
    }
}
