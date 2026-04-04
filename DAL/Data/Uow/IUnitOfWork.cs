using DAL.Repositories.BouquetFlowers;
using DAL.Repositories.Bouquets;
using DAL.Repositories.Categories;
using DAL.Repositories.Colors;
using DAL.Repositories.Flowers;
using DAL.Repositories.OrderBouquets;
using DAL.Repositories.Orders;
using DAL.Repositories.WrappingPapers;

namespace DAL.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IBouquetRepository BouquetRepository { get; }
        IFlowerRepository FlowerRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderBouquetRepository OrderBouquetRepository { get; }
        IBouquetFlowerRepository BouquetFlowerRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IColorRepository ColorRepository { get; }
        IWrappingPaperRepository WrappingPaperRepository { get; }
        Task CompleteAsync();
    }
}
