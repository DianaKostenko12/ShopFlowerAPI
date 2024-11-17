using DAL.Filters;
using DAL.Models;
using DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Bouquets
{
    public interface IBouquetRepository : IBaseRepository<Bouquet>
    {
        Task<List<Bouquet>> GetBouquetsByUserIdAsync(int userId);
        Task<List<Bouquet>> GetBouquetsByFilterAsync(BouquetFilterView view);
        Task<Bouquet> GetBouquetByIdAsync(int id);
    }
}
