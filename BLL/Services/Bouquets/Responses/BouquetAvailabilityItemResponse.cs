namespace BLL.Services.Bouquets.Responses
{
    public class BouquetAvailabilityItemResponse
    {
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int RequiredFlowerCount { get; set; }
        public int AvailableFlowerCount { get; set; }
        public bool IsAvailable { get; set; }
    }
}
