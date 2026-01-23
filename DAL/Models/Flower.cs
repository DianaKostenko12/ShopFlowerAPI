namespace DAL.Models
{
    public class Flower
    { 
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int FlowerCount { get; set; }
        public decimal FlowerCost { get; set; }
        public string PhotoFileName { get; set; }
        public string Color { get; set; }
        public string Category { get; set; }
        public int HeadSizeCm { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<BouquetFlower> BouquetsFlowers { get; set; }
    }
}
