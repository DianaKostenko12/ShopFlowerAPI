using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models.Orders;

namespace DAL.Models
{
    public class OrderBouquet
    {
        public int BouquetId { get; set; }
        public int OrderId { get; set; }
        public int BouquetCount { get; set; }
        public Bouquet Bouquet { get; set; }
        public Order Order { get; set; }
    }
}
