namespace DAL.Models
{
    public class BouquetFlower
    {
        public int FlowerId { get; set; }
        public int BouquetId { get; set; }
        public int FlowerCount { get; set; }
        public Flower Flower { get; set; }
        public Bouquet Bouquet { get; set; }
    }
}
