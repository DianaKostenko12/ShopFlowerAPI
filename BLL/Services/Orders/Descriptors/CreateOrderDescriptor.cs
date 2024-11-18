using BLL.Services.Bouquets.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Orders.Descriptors
{
    public class CreateOrderDescriptor
    {
       public string DeliveryStreet {  get; set; }
       public List<BouquetQuantityDescriptor> Bouquets { get; set; }
    }
}
