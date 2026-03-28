namespace BLL.Services.Flowers.Descriptors
{
    public class UpdateFlowerDescriptor
    {
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int FlowerCount { get; set; }
        public decimal FlowerCost { get; set; }
    }
}
