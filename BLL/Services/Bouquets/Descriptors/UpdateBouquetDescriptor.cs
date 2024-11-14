using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Bouquets.Descriptors
{
    public class UpdateBouquetDescriptor
    {
        public int BouquetId {get; set;}
        public string BouquetName { get; set; }
        public string BouquetDescription { get; set; }
        public int FlowerCount { get; set; }
        public List<int> FlowerIds { get; set; }
    }
}
