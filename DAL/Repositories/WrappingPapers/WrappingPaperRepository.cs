using DAL.Data;
using DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.WrappingPapers
{
    public class WrappingPaperRepository : BaseRepository<Models.WrappingPaper>, IWrappingPaperRepository
    {
        public WrappingPaperRepository(DataContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Models.WrappingPaper>> FindAllAsync()
        {
            return await Sourse
                .Include(wp => wp.Color)
                .ToListAsync();
        }

        public override async Task<Models.WrappingPaper> FindAsync(int id)
        {
            return await Sourse
                .Include(wp => wp.Color)
                .FirstOrDefaultAsync(wp => wp.WrappingPaperId == id);
        }
    }
}
