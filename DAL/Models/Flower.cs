using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Flower
    {
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int FlowerCount { get; set; }
        public decimal FlowerCost { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<BouquetFlower> BouquetsFlowers { get; set; }
    }
}
