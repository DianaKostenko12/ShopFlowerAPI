using DAL.Data;
using DAL.Filters;
using DAL.Models;
using DAL.Repositories.Base;
using DAL.Repositories.Flowers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task<List<Bouquet>> GetBouquetsByFilterAsync(BouquetFilterView view)
        {
            var query = Sourse
                .Include(b => b.BouquetsFlowers)
                .ThenInclude(bf => bf.Flower)
                .AsQueryable();

            if (view.MinPrice > 0 || view.MaxPrice > 0)
            {
                query = query.Where(b => b.BouquetsFlowers
                    .Sum(bf => bf.Flower.FlowerCost * bf.FlowerCount) >= view.MinPrice &&
                    b.BouquetsFlowers.Sum(bf => bf.Flower.FlowerCost * bf.FlowerCount) <= view.MaxPrice);
            }

            if (view.FlowerIds != null && view.FlowerIds.Any())
            {
                query = query.Where(b => b.BouquetsFlowers
                    .Any(bf => view.FlowerIds.Contains(bf.FlowerId)));
            }

            return await query.ToListAsync();
        }
    }
}
