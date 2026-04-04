using Microsoft.AspNetCore.Http;

namespace FlowerShopApi.DTOs.Bouquets
{
    public class CreateBouquetRequest
    {
        public string BouquetName { get; set; }
        public string BouquetDescription { get; set; }
        public IFormFile Photo { get; set; }
        public List<FlowerQuantityRequest> Flowers { get; set; }
    }
}
