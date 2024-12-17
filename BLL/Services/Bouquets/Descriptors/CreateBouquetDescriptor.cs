using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Bouquets.Descriptors
{
    public class CreateBouquetDescriptor
    {
        public string BouquetName { get; set; }
        public string BouquetDescription { get; set; }
        public IFormFile Photo { get; set; }
        public List<FlowerQuantityDescriptor> Flowers { get; set; }
    }
}
