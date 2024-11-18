using BLL.Services.Bouquets.Descriptors;
using BLL.Services.Orders.Descriptors;
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
    }
}
