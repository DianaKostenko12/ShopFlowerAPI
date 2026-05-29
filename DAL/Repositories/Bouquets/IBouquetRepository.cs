using DAL.Filters;
using DAL.Models;
using DAL.Repositories.Base;

namespace DAL.Repositories.Bouquets
{
    public interface IBouquetRepository : IBaseRepository<Bouquet>
    {
        Task<List<Bouquet>> GetBouquetsByUserIdAsync(int userId);
        Task<List<BouquetFilterResult>> GetBouquetsByFilterAsync(BouquetFilterView view, int? userId);
        Task<decimal> GetBouquetPriceAsync(int bouquetId);
        Task<Bouquet> GetBouquetWithUserAsync(int bouquetId);
        Task<Bouquet> GetBouquetWithFlowersAsync(int bouquetId);
    }
}
