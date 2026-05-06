using DAL.Data;
using DAL.Filters;
using DAL.Models;
using DAL.Repositories.Base;
using DAL.Repositories.Flowers;
using Microsoft.EntityFrameworkCore;
namespace DAL.Repositories.Bouquets
{
    public class BouquetRepository : BaseRepository<Bouquet>, IBouquetRepository
    {
        public BouquetRepository(DataContext context) : base(context)
        {
            
        }

        public override async Task<Bouquet> FindAsync(int bouquetId)
        {
            return await Sourse
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BouquetId == bouquetId && !b.IsDeleted);
        }

        public override async Task RemoveAsync(Bouquet bouquet)
        {
            bouquet.IsDeleted = true;
        }

        public async Task<List<Bouquet>> GetBouquetsByUserIdAsync(int userId)
        {
            return await Sourse
                .Where(b => b.IsDeleted == false &&
                            b.User != null &&
                            b.User.Id == userId &&
                            b.BouquetsFlowers.Any(bf => bf.Flower != null && bf.Flower.IsDeleted == false))
                .Include(b => b.User)
                .Include(b => b.BouquetsFlowers)
                    .ThenInclude(bf => bf.Flower)
                        .ThenInclude(f => f.Color)
                .Include(b => b.BouquetsFlowers)
                    .ThenInclude(bf => bf.Flower)
                        .ThenInclude(f => f.Category)
                .ToListAsync();
        }

        public async Task<List<Bouquet>> GetBouquetsByFilterAsync(BouquetFilterView view)
        {
            var query = Sourse
                .Where(b => b.IsDeleted == false)
                .Include(b => b.BouquetsFlowers)
                .ThenInclude(bf => bf.Flower)
                    .ThenInclude(f => f.Color)
                .Include(b => b.BouquetsFlowers)
                .ThenInclude(bf => bf.Flower)
                    .ThenInclude(f => f.Category)
                .Where(f => f.IsDeleted == false)
                .Include(b => b.User)
                .AsQueryable();

            if (view.MinPrice > 0 || view.MaxPrice > 0)
            {
                query = query.Where(b => b.BouquetsFlowers
                    .Sum(bf => bf.Flower.FlowerCost * bf.FlowerCount) >= view.MinPrice &&
                    b.BouquetsFlowers.Sum(bf => bf.Flower.FlowerCost * bf.FlowerCount) <= view.MaxPrice);
            }

            if (view.CategoriesIds != null && view.CategoriesIds.Any())
            {
                query = query.Where(b => b.BouquetsFlowers
                    .Any(bf => view.CategoriesIds.Contains(bf.Flower.CategoryId)));
            }

            return await query.ToListAsync();
        }

        public async Task<decimal> GetBouquetPriceAsync(int bouquetId)
        {
            return await Sourse
                .Where(b => !b.IsDeleted && b.BouquetId == bouquetId)
                .Select(b => b.BouquetsFlowers.Sum(bf => bf.Flower.FlowerCost * bf.FlowerCount))
                .FirstOrDefaultAsync();
        }

        public async Task<Bouquet> GetBouquetWithFlowersAsync(int bouquetId)
        {
            return await Sourse
                .Where(b => !b.IsDeleted && b.BouquetId == bouquetId)
                .Include(b => b.BouquetsFlowers)
                    .ThenInclude(bf => bf.Flower)
                .FirstOrDefaultAsync();
        }
    }
}
