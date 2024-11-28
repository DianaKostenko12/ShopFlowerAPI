using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Bouquet
    {
        public int BouquetId { get; set; }
        public string BouquetName { get; set; }
        public string BouquetDescription { get; set; }
        public bool IsDeleted { get; set; }
        public User User { get; set; }
        public ICollection<BouquetFlower> BouquetsFlowers { get; set; }
        public ICollection<OrderBouquet> OrderBouquets { get; set; }

        public decimal Price => BouquetsFlowers?.Sum(bf => bf.Flower.FlowerCost * bf.FlowerCount) ?? 0;
    }
}
