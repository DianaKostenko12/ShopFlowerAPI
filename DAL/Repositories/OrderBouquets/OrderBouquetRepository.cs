using DAL.Data;
using DAL.Models;
using DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.OrderBouquets
{
    public class OrderBouquetRepository : BaseRepository<OrderBouquet>, IOrderBouquetRepository
    {
        public OrderBouquetRepository(DataContext context) : base(context)
        {
        }

        public async Task<List<OrderBouquet>> GetByOrderIdAsync(int orderId)
        {
            return await Sourse.Where(ob => ob.OrderId == orderId).ToListAsync();
        }
    }
}
