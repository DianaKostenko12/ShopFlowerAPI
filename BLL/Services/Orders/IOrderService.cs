using BLL.Services.Orders.Descriptors;
using DAL.Models.Orders;

namespace BLL.Services.Orders
{
    public interface IOrderService
    {
        Task<int> AddOrderAsync(CreateOrderDescriptor descriptor, int userId);
        Task AssembleOrderBouquetsAsync(int orderId, List<BouquetQuantityDescriptor> bouquets);
        Task ChangeOrderStatus(int orderId, OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
        Task<IEnumerable<Order>> GetOrders();
    }
}
