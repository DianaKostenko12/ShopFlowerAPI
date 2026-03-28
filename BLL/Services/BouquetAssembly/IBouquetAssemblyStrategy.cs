using BLL.Services.BouquetAssembly.Descriptors;
using BLL.Services.BouquetAssembly.Responses;

namespace BLL.Services.BouquetAssembly
{
    public interface IBouquetAssemblyStrategy
    {
        AssemblyResult Assemble(AssemblyPlanDescriptor plan);
    }
}
