using DAL.Models;
using DAL.Models.Orders;
using DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Orders
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
    }
}
