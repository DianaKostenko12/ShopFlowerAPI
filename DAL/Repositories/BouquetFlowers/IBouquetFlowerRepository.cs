using DAL.Models;
using DAL.Repositories.Base;

namespace DAL.Repositories.BouquetFlowers
{
    public interface IBouquetFlowerRepository : IBaseRepository<BouquetFlower>
    {
        Task<List<BouquetFlower>> GetByBouquetIdAsync(int bouquetId);
    }
}
