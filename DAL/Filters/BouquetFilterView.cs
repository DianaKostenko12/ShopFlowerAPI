namespace DAL.Filters
{
    public class BouquetFilterView
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<int> CategoriesIds { get; set; }
        public List<string> ShapesList { get; set; }
        public List<string> ColorsList { get; set; }
    }
}
