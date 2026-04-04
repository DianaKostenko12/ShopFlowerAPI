namespace DAL.Models.Flowers
{
    public class Color
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public ICollection<Flower> Flowers { get; set; }
    }
}
