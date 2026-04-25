namespace BLL.Services.Bouquets.Responses
{
    public class BouquetAvailabilityResponse
    {
        public int BouquetId { get; set; }
        public int BouquetCount { get; set; }
        public bool IsAvailable { get; set; }
        public List<BouquetAvailabilityItemResponse> Flowers { get; set; } = new();
    }
}
