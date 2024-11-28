using DAL.Data;
using DAL.Models.Orders;
using DAL.Repositories.Base;
using DAL.Repositories.OrderBouquets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Orders
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(DataContext context) : base(context)
        {
            
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await Sourse
                .Where(o => o.User.Id == userId)
                    .Include(o => o.User)
                        .Include(o => o.OrderBouquets)
                            .ThenInclude(ob => ob.Bouquet)
                                .ThenInclude(b => b.BouquetsFlowers)
                                    .ThenInclude(bf => bf.Flower)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Order>> FindAllAsync()
        {
            return await Sourse
                    .Include(o => o.User)
                    .Include(o => o.OrderBouquets)
                        .ThenInclude(ob => ob.Bouquet)
                            .ThenInclude(b => b.BouquetsFlowers)
                                .ThenInclude(bf => bf.Flower)
                    .ToListAsync();
        }
    }
}
