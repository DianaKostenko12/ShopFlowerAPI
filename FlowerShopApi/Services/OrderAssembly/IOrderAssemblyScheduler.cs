using BLL.Services.Orders.Descriptors;

namespace FlowerShopApi.Services.OrderAssembly
{
    public interface IOrderAssemblyScheduler
    {
        void ScheduleAssembly(int orderId, IEnumerable<BouquetQuantityDescriptor> bouquets);
    }
}
