using DAL.Data;
using DAL.Models;
using DAL.Repositories.Base;
using DAL.Repositories.Flowers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Bouquets
{
    public class BouquetRepository : BaseRepository<Bouquet>, IBouquetRepository
    {
        public BouquetRepository(DataContext context) : base(context)
        {
            
        }

        public override async Task<Bouquet> FindAsync(int bouquetId)
        {
            return await Sourse.FirstOrDefaultAsync(b => b.BouquetId == bouquetId && !b.IsDeleted);
        }

        public override async Task<IEnumerable<Bouquet>> FindAllAsync()
        {
            return await Sourse.Where(b => b.IsDeleted == false).ToListAsync();
        }

        public override async Task RemoveAsync(Bouquet bouquet)
        {
            bouquet.IsDeleted = true;
        }

        public async Task<List<Bouquet>> GetBouquetsByUserIdAsync(int userId)
        {
            return await Sourse
                .Include(b => b.User)
                .Where(b => b.User.Id == userId).ToListAsync();
        }

        public async Task<List<Bouquet>> GetPostsByFilterAsync(BouquetFilterRequest request)
        {
            var query = await Sourse
                .Include(b => b.BouquetsFlowers)
                .ThenInclude(bf => bf.Flower)
                .ToListAsync();
                
        }
    }
}
