using DAL.Repositories.BouquetFlowers;
using DAL.Repositories.Bouquets;
using DAL.Repositories.Flowers;
using DAL.Repositories.OrderBouquets;
using DAL.Repositories.Orders;

namespace DAL.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IBouquetRepository BouquetRepository { get; }
        IFlowerRepository FlowerRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderBouquetRepository OrderBouquetRepository { get; }
        IBouquetFlowerRepository BouquetFlowerRepository { get; }

        Task CompleteAsync();
    }
}
