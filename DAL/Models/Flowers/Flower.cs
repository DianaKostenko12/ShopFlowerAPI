namespace DAL.Models.Flowers
{
    public class Flower
    {
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public int FlowerCount { get; set; }
        public decimal FlowerCost { get; set; }
        public string PhotoFileName { get; set; }
        public int? ColorId { get; set; }
        public Color Color { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public int HeadSizeCm { get; set; }
        public double StemThicknessMm { get; set; }
        public StemType StemKind { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<BouquetFlower> BouquetsFlowers { get; set; }
    }
}
