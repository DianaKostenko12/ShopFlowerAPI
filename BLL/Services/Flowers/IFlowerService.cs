using BLL.Services.Flowers.Descriptors;
using DAL.Models;

namespace BLL.Services.Flowers
{
    public interface IFlowerService
    {
        Task AddFlowerAsync(CreateFlowerDescriptor descriptor);
        Task UpdateFlowerAsync(UpdateFlowerDescriptor descriptor);
        Task DeleteFlowerAsync(int flowerId);
        Task<Flower> GetFlowerByIdAsync(int floweId);
        Task<IEnumerable<Flower>> GetFlowersAsync();
    }
}
