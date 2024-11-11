using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Flowers.Descriptors
{
    public class UpdateFlowerDescriptor
    {
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int FlowerCount { get; set; }
        public float FlowerCost { get; set; }
    }
}
