using DAL.Data;
using DAL.Models;
using DAL.Repositories.Base;
using DAL.Repositories.Bouquets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
