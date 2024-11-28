using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.OrderBouquets
{
    public interface IOrderBouquetService
    {
        public Task<List<OrderBouquet>> GetByOrderIdAsync(int orderId);
    }
}
