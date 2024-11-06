using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class BouquetFlower
    {
        public int FlowerId { get; set; }
        public int BouquetId { get; set; }
        public int FlowerCount { get; set; }
        public Flower Flower { get; set; }
        public Bouquet Bouquet { get; set; }
    }
}
