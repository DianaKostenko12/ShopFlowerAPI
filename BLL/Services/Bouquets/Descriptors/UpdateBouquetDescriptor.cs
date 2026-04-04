namespace BLL.Services.Bouquets.Descriptors
{
    public class UpdateBouquetDescriptor
    {
        public int BouquetId {get; set;}
        public string BouquetName { get; set; }
        public string BouquetDescription { get; set; }
        public List<FlowerQuantityDescriptor> Flowers { get; set; }
    }
}
