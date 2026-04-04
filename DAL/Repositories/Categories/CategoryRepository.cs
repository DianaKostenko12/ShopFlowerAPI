using DAL.Data;
using DAL.Models.Flowers;
using DAL.Repositories.Base;

namespace DAL.Repositories.Categories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DataContext context) : base(context)
        {
        }
    }
}
