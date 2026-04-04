using DAL.Data;
using DAL.Models;
using DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.BouquetFlowers
{
    public class BouquetFlowerRepository : BaseRepository<BouquetFlower>, IBouquetFlowerRepository
    {
        public BouquetFlowerRepository(DataContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<List<BouquetFlower>> GetByBouquetIdAsync(int bouquetId)
        {
            return await Sourse.Where(bf => bf.BouquetId == bouquetId).ToListAsync();
        }
    }
}
