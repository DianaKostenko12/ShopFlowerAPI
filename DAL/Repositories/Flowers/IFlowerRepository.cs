using DAL.Models.Flowers;
using DAL.Repositories.Base;

namespace DAL.Repositories.Flowers
{
    public interface IFlowerRepository : IBaseRepository<Flower>
    {
    }
}
