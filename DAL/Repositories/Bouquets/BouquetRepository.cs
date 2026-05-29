using DAL.Data;
using DAL.Filters;
using DAL.Models;
using DAL.Models.Flowers;
using DAL.Repositories.Base;
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
                .TagWith("BouquetRepository.FindAsync")
                .FirstOrDefaultAsync(b => b.BouquetId == bouquetId && !b.IsDeleted);
        }

        public override async Task RemoveAsync(Bouquet bouquet)
        {
            bouquet.IsDeleted = true;
        }

        public async Task<List<Bouquet>> GetBouquetsByUserIdAsync(int userId)
        {
            var query = Sourse
                .TagWith("BouquetRepository.GetBouquetsByUserIdAsync")
                .AsNoTracking()
                .AsSplitQuery()
                .Where(b => b.IsDeleted == false &&
                            b.UserId == userId &&
                            b.BouquetsFlowers.Any(bf => bf.Flower != null && bf.Flower.IsDeleted == false));

            return await SelectBouquetSummaryFields(query)
                .ToListAsync();
        }

        public async Task<List<BouquetFilterResult>> GetBouquetsByFilterAsync(BouquetFilterView view, int? userId)
        {
            view ??= new BouquetFilterView();

            var query = Sourse
                .TagWith("BouquetRepository.GetBouquetsByFilterAsync")
                .AsNoTracking()
                .AsSplitQuery()
                .Where(b => b.IsDeleted == false)
                .Where(b => b.BouquetsFlowers.Any(bf => bf.Flower != null && bf.Flower.IsDeleted == false))
                .AsQueryable();

            if (view.MinPrice > 0)
            {
                query = query.Where(b => b.BouquetsFlowers
                    .Where(bf => bf.Flower != null && bf.Flower.IsDeleted == false)
                    .Sum(bf => bf.Flower.FlowerCost * bf.FlowerCount) >= view.MinPrice);
            }

            if (view.MaxPrice > 0)
            {
                query = query.Where(b => b.BouquetsFlowers
                    .Where(bf => bf.Flower != null && bf.Flower.IsDeleted == false)
                    .Sum(bf => bf.Flower.FlowerCost * bf.FlowerCount) <= view.MaxPrice);
            }

            if (view.CategoriesIds != null && view.CategoriesIds.Any())
            {
                query = query.Where(b => b.BouquetsFlowers
                    .Any(bf => bf.Flower != null &&
                               bf.Flower.IsDeleted == false &&
                               view.CategoriesIds.Contains(bf.Flower.CategoryId)));
            }

            if (view.ShapesList != null && view.ShapesList.Any())
            {
                var shapes = view.ShapesList
                    .Where(shape => !string.IsNullOrWhiteSpace(shape))
                    .Select(shape => shape.Trim())
                    .Distinct()
                    .ToList();

                if (shapes.Any())
                {
                    query = query.Where(b => shapes.Contains(b.Shape));
                }
            }

            var adminUserIds = _context.UserRoles
                .Join(
                    _context.Roles,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => new { userRole.UserId, RoleName = role.Name })
                .Where(userRoleWithRole => userRoleWithRole.RoleName == Roles.Admin)
                .Select(userRoleWithRole => (int?)userRoleWithRole.UserId);

            query = query.Where(b =>
                adminUserIds.Contains(b.UserId) ||
                (userId.HasValue && b.UserId == userId.Value));

            return await SelectFilteredBouquetFields(query)
                .ToListAsync();
        }

        public async Task<decimal> GetBouquetPriceAsync(int bouquetId)
        {
            return await Sourse
                .TagWith("BouquetRepository.GetBouquetPriceAsync")
                .Where(b => !b.IsDeleted && b.BouquetId == bouquetId)
                .Select(b => b.BouquetsFlowers.Sum(bf => bf.Flower.FlowerCost * bf.FlowerCount))
                .FirstOrDefaultAsync();
        }

        public async Task<Bouquet> GetBouquetWithUserAsync(int bouquetId)
        {
            return await Sourse
                .AsNoTracking()
                .TagWith("BouquetRepository.GetBouquetWithUserAsync")
                .Where(b => b.BouquetId == bouquetId && !b.IsDeleted)
                .Select(b => new Bouquet
                {
                    BouquetId = b.BouquetId,
                    User = b.User == null
                        ? null
                        : new User
                        {
                            Id = b.User.Id
                        }
                })
                .FirstOrDefaultAsync();
        }

        private static IQueryable<Bouquet> SelectBouquetSummaryFields(IQueryable<Bouquet> query)
        {
            return query.Select(b => new Bouquet
            {
                BouquetId = b.BouquetId,
                BouquetName = b.BouquetName,
                BouquetDescription = b.BouquetDescription,
                BouquetsFlowers = b.BouquetsFlowers
                    .Where(bf => bf.Flower != null && bf.Flower.IsDeleted == false)
                    .Select(bf => new BouquetFlower
                    {
                        BouquetId = bf.BouquetId,
                        FlowerId = bf.FlowerId,
                        FlowerCount = bf.FlowerCount,
                        Role = bf.Role,
                        Flower = new Flower
                        {
                            FlowerId = bf.Flower.FlowerId,
                            FlowerCost = bf.Flower.FlowerCost
                        }
                    })
                    .ToList()
            });
        }

        private static IQueryable<BouquetFilterResult> SelectFilteredBouquetFields(IQueryable<Bouquet> query)
        {
            return query.Select(b => new BouquetFilterResult
            {
                BouquetId = b.BouquetId,
                BouquetName = b.BouquetName,
                BouquetDescription = b.BouquetDescription,
                Shape = b.Shape,
                Price = b.BouquetsFlowers
                    .Where(bf => bf.Flower != null && bf.Flower.IsDeleted == false)
                    .Sum(bf => bf.Flower.FlowerCost * bf.FlowerCount),
                ColorNames = b.BouquetsFlowers
                    .Where(bf => bf.Flower != null && bf.Flower.IsDeleted == false)
                    .Select(bf => bf.Flower.Color.ColorName)
                    .ToList()
            });
        }

        public async Task<Bouquet> GetBouquetWithFlowersAsync(int bouquetId)
        {
            return await Sourse
                .TagWith("BouquetRepository.GetBouquetWithFlowersAsync")
                .Where(b => !b.IsDeleted && b.BouquetId == bouquetId)
                .Include(b => b.BouquetsFlowers)
                    .ThenInclude(bf => bf.Flower)
                .FirstOrDefaultAsync();
        }
    }
}
