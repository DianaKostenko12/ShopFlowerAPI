namespace DAL.Models.Flowers
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Flower> Flowers { get; set; }
    }
}
