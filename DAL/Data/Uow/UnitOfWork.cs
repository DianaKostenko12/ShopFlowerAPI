using DAL.Repositories.BouquetFlowers;
using DAL.Repositories.Bouquets;
using DAL.Repositories.Flowers;
using DAL.Repositories.OrderBouquets;
using DAL.Repositories.Orders;

namespace DAL.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private IBouquetRepository _bouquetRepository;
        private IFlowerRepository _flowerRepository;
        private IOrderRepository _orderRepository;
        private IOrderBouquetRepository _orderBouquetRepository;
        private IBouquetFlowerRepository _bouquetFlowerRepository;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        public IBouquetRepository BouquetRepository
        {
            get
            {
                return _bouquetRepository ??= new BouquetRepository(_context);
            }
        }

        public IFlowerRepository FlowerRepository
        {
            get
            {
                return _flowerRepository ??= new FlowerRepository(_context);
            }
        }

        public IOrderRepository OrderRepository
        {
            get
            {
                return _orderRepository ??= new OrderRepository(_context);
            }
        }

        public IOrderBouquetRepository OrderBouquetRepository
        {
            get
            {
                return _orderBouquetRepository ??= new OrderBouquetRepository(_context);
            }
        }

        public IBouquetFlowerRepository BouquetFlowerRepository
        {
            get
            {
                return _bouquetFlowerRepository ??= new BouquetFlowerRepository(_context);
            }
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
