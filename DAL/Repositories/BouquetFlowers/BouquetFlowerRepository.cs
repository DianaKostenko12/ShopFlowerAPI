using DAL.Data;
using DAL.Models;
using DAL.Repositories.Base;
using DAL.Repositories.Orders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.BouquetFlowers
{
    public class BouquetFlowerRepository : BaseRepository<BouquetFlower>, IBouquetFlowerRepository
    {
        public BouquetFlowerRepository(DataContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<List<BouquetFlower>> GetByBouquetIdAsync(int bouquetId)
        {
            return await Sourse.Where(bf => bf.BouquetId == bouquetId).ToListAsync();
        }
    }
}
