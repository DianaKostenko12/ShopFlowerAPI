using BLL.Services.Bouquets.Descriptors;
using BLL.Services.Orders.Descriptors;
using DAL.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Orders
{
    public interface IOrderService
    {
        Task AddOrderAsync(CreateOrderDescriptor descriptor, int userId);
        Task ChangeOrderStatus(int orderId, OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
        Task<IEnumerable<Order>> GetOrders();
    }
}
