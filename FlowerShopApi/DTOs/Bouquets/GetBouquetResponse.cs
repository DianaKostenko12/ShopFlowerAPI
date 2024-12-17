namespace FlowerShopApi.DTOs.Bouquets
{
    public class GetBouquetResponse
    { 
        public int BouquetId { get; set; }
        public string BouquetName { get; set; }
        public string BouquetDescription { get; set; }
        public string PhotoFileName { get; set; }
        public decimal Price { get; set; }
    }
}
