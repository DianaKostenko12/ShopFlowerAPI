using BLL.Services.BouquetAssembly.Descriptors;
using BLL.Services.BouquetAssembly.Responses;

namespace BLL.Services.BouquetAssembly
{
    public interface IBouquetAssembly
    {
        AssemblyResult ExecuteAssembly(AssemblyPlanDescriptor plan);
    }
}
