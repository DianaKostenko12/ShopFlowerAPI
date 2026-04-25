using BLL.Services.Bouquets.Descriptors;
using BLL.Services.Bouquets.Responses;
using DAL.Filters;
using DAL.Models;

namespace BLL.Services.Bouquets
{
    public interface IBouquetService
    {
        Task AddBouquetAsync(CreateBouquetDescriptor descriptor, int userId);
        Task<BouquetAvailabilityResponse> CheckAvailabilityAsync(int bouquetId, int bouquetCount);
        Task DeleteBouquetAsync(int bouquetId, int userId);
        Task<Bouquet> GetBouquetByIdAsync(int bouquetId);
        Task<List<Bouquet>> GetBouquetsByUserIdAsync(int userId);
        Task<List<Bouquet>> GetBouquetsByFilterAsync(BouquetFilterView view, int? userId);
        Task<bool> IsUserBouquetOwnerAsync(int bouquetId, int userId);
    }
}
