using DAL.Models;
using DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.BouquetFlowers
{
    public interface IBouquetFlowerRepository : IBaseRepository<BouquetFlower>
    {
        Task<List<BouquetFlower>> GetByBouquetIdAsync(int bouquetId);
    }
}
