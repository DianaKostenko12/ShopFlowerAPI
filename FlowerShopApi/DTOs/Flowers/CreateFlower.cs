using DAL.Models.Flowers;

namespace FlowerShopApi.DTOs.Flowers
{
    public class CreateFlower
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
