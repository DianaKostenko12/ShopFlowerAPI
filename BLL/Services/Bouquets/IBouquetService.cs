using BLL.Services.Bouquets.Descriptors;
using BLL.Services.Flowers.Descriptors;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Bouquets
{
    public interface IBouquetService
    {
        Task AddBouquetAsync(CreateBouquetDescriptor descriptor, int userId);
        Task UpdateBouquetAsync(UpdateBouquetDescriptor descriptor);
        Task DeleteBouquetAsync(int bouquetId);
        Task<List<Bouquet>> GetBouquetByUserIdAsync(int userId);
        Task<List<Bouquet>> GetBouquetsByFilterAsync(BouquetFilterView view);
    }
}
