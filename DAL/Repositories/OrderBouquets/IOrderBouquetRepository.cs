using DAL.Models;
using DAL.Repositories.Base;

namespace DAL.Repositories.OrderBouquets
{
    public interface IOrderBouquetRepository : IBaseRepository<OrderBouquet>
    {
        Task<List<OrderBouquet>> GetByOrderIdAsync(int orderId);
    }
}
