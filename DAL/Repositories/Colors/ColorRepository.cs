using DAL.Data;
using DAL.Models.Flowers;
using DAL.Repositories.Base;

namespace DAL.Repositories.Colors
{
    public class ColorRepository : BaseRepository<Color>, IColorRepository
    {
        public ColorRepository(DataContext context) : base(context)
        {
        }
    }
}
