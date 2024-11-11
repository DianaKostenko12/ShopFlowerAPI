namespace FlowerShopApi.DTOs.Flowers
{
    public class FlowerRequest
    {
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int FlowerCount { get; set; }
        public float FlowerCost { get; set; }
    }
}
