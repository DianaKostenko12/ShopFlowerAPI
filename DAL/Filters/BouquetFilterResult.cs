namespace DAL.Filters
{
    public class BouquetFilterResult
    {
        public int BouquetId { get; set; }
        public string BouquetName { get; set; }
        public string BouquetDescription { get; set; }
        public string Shape { get; set; }
        public decimal Price { get; set; }
        public List<string> ColorNames { get; set; } = [];
    }
}
