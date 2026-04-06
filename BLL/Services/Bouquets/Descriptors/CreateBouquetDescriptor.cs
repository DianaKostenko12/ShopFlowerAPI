using Microsoft.AspNetCore.Http;

namespace BLL.Services.Bouquets.Descriptors
{
    public class CreateBouquetDescriptor
    {
        public string BouquetName { get; set; }
        public string BouquetDescription { get; set; }
        public int WrappingPaperId { get; set; }
        public string Shape { get; set; }
        public IFormFile Photo { get; set; }
        public byte[] PhotoBytes { get; set; }
        public string PhotoContentType { get; set; }
        public List<FlowerQuantityDescriptor> Flowers { get; set; }
    }
}
