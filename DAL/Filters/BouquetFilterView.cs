using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Filters
{
    public class BouquetFilterView
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<int> FlowerIds { get; set; }
    }
}
