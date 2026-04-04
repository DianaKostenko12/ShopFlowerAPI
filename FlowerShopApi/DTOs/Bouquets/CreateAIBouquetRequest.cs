namespace FlowerShopApi.DTOs.Bouquets
{
    public class CreateAIBouquetRequest
    {
        public string BouquetName { get; set; }
        public string BouquetDescription { get; set; }
        public byte[] PhotoBytes { get; set; }
        public string PhotoContentType { get; set; }
        public List<FlowerQuantityRequest> Flowers { get; set; }
    }
}
