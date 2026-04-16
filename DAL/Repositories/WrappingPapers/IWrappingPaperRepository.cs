using DAL.Models;
using DAL.Repositories.Base;

namespace DAL.Repositories.WrappingPapers
{
    public interface IWrappingPaperRepository : IBaseRepository<WrappingPaper>
    {
        Task<WrappingPaper> FindByVariantAsync(WrappingPaperType type, int colorId, WrappingPaperPattern pattern);
    }
}
