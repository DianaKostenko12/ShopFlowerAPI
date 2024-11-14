namespace FlowerShopApi.Requests
{
    public class BouquetFilterRequest
    {
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }
        public List<int> FlowerIds { get; set; }
    }
}
