using DAL.Data;
using DAL.Models;
using DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Flowers
{
    public class FlowerRepository : BaseRepository<Flower>, IFlowerRepository
    {
        public FlowerRepository(DataContext context) : base(context)
        {
        }

        public override async Task<Flower> FindAsync(int floweId)
        {
            return await Sourse.FirstOrDefaultAsync(f => f.FlowerId == floweId && !f.IsDeleted);
        }

        public override async Task<IEnumerable<Flower>> FindAllAsync()
        {
            return await Sourse.Where(f => f.IsDeleted == false).ToListAsync();
        }

        public override async Task RemoveAsync(Flower flower)
        {
            var flowerToRemove = await Sourse.FirstOrDefaultAsync(f => f.FlowerId == flower.FlowerId);

            if (flowerToRemove != null)
            {
                flowerToRemove.IsDeleted = true;
            }
        }

       
    }
}
