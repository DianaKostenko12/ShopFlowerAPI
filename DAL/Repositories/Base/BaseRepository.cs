using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected DataContext _context;
        protected DbSet<TEntity> Sourse;

        public BaseRepository(DataContext context)
        {
            _context = context;
            Sourse = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await Sourse.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await Sourse.AddRangeAsync(entities);
        }

        public virtual Task RemoveAsync(TEntity entity)
        {
            Sourse.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            Sourse.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return await Sourse.ToListAsync();
        }

        public virtual async Task<TEntity> FindAsync(int id)
        {
            return await Sourse.FindAsync(id);
        }

        //public async Task<bool> Save()
        //{
        //    var saved = await _context.SaveChangesAsync();
        //    return saved > 0 ? true : false;
        //}
    }
}
