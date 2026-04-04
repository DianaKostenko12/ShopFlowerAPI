using DAL.Models.Flowers;
using Microsoft.AspNetCore.Http;

namespace BLL.Services.Flowers.Descriptors
{
    public class CreateFlowerDescriptor
    {
        public string FlowerName { get; set; }
        public int FlowerCount { get; set; }
        public IFormFile Photo { get; set; }
        public float FlowerCost { get; set; }
        public int? ColorId { get; set; }
        public int? CategoryId { get; set; }
        public int HeadSizeCm { get; set; }
        public double StemThicknessMm { get; set; }
        public StemType StemKind { get; set; }
    }
}
