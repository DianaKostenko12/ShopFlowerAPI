using DAL.Data;
using DAL.Repositories.Base;

namespace DAL.Repositories.WrappingPapers
{
    public class WrappingPaperRepository : BaseRepository<Models.WrappingPaper>, IWrappingPaperRepository
    {
        public WrappingPaperRepository(DataContext context) : base(context)
        {
        }
    }
}
