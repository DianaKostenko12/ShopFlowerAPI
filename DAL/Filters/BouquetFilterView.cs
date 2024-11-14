using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Filters
{
    public class BouquetFilterView
    {
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }
        public List<int> FlowerIds { get; set; }
    }
}
