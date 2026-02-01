using DAL.Models.Orders;
using DAL.Repositories.Base;

namespace DAL.Repositories.Orders
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
    }
}
