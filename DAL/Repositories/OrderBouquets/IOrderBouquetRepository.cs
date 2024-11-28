using DAL.Models;
using DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.OrderBouquets
{
    public interface IOrderBouquetRepository : IBaseRepository<OrderBouquet>
    {
        Task<List<OrderBouquet>> GetByOrderIdAsync(int orderId);
    }
}
