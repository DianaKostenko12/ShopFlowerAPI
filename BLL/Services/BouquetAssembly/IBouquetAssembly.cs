using BLL.Services.BouquetAssembly.Descriptors;
using BLL.Services.BouquetAssembly.DTOs;

namespace BLL.Services.BouquetAssembly
{
    public interface IBouquetAssembly
    {
        AssemblyResult ExecuteAssembly(AssemblyPlanDescriptor plan);
    }
}
