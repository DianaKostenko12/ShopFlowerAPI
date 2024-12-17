namespace FlowerShopApi.DTOs.Flowers
{
    public class CreateFlower
    {
        public string FlowerName { get; set; }
        public int FlowerCount { get; set; }
        public IFormFile Photo { get; set; }
        public float FlowerCost { get; set; }
    }
}
