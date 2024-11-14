using DAL.Data;
using DAL.Models;
using DAL.Repositories.Base;
using DAL.Repositories.OrderBouquets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Orders
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(DataContext context) : base(context)
        {
            
        }
    }
}
