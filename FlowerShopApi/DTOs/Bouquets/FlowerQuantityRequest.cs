using DAL.Models;

namespace FlowerShopApi.DTOs.Bouquets
{
    public class FlowerQuantityRequest
    {
        public int FlowerId { get; set; }
        public int FlowerCount { get; set; }
        public FlowerRole Role { get; set; }
    }
}
