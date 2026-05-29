using BLL.Services.Orders;
using BLL.Services.Orders.Descriptors;

namespace FlowerShopApi.Services.OrderAssembly
{
    public class OrderAssemblyScheduler : IOrderAssemblyScheduler
    {
        private static readonly TimeSpan AssemblyDelay = TimeSpan.FromMinutes(1);

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OrderAssemblyScheduler> _logger;

        public OrderAssemblyScheduler(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<OrderAssemblyScheduler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public void ScheduleAssembly(int orderId, IEnumerable<BouquetQuantityDescriptor> bouquets)
        {
            var bouquetsSnapshot = (bouquets ?? Enumerable.Empty<BouquetQuantityDescriptor>())
                .Select(b => new BouquetQuantityDescriptor
                {
                    BouquetId = b.BouquetId,
                    BouquetCount = b.BouquetCount
                })
                .ToList();

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(AssemblyDelay);

                    using var scope = _serviceScopeFactory.CreateScope();
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    await orderService.AssembleOrderBouquetsAsync(orderId, bouquetsSnapshot);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to assemble bouquets for order {OrderId}.", orderId);
                }
            });
        }
    }
}
