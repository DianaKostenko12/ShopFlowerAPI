using DAL.Data;
using DAL.Models;
using DAL.Repositories.Base;
using DAL.Repositories.Bouquets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.OrderBouquets
{
    public class OrderBouquetRepository : BaseRepository<OrderBouquet>, IOrderBouquetRepository
    {
        public OrderBouquetRepository(DataContext context) : base(context)
        {
        }
    }
}
