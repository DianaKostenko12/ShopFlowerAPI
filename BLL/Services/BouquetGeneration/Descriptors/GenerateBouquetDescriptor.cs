namespace BLL.Services.BouquetGeneration.Descriptors
{
    public class GenerateBouquetDescriptor
    {
        public string Category { get; set; }
        public List<string> ColorScheme { get; set; }
        public string BouquetShape { get; set; }
        public decimal Budget { get; set; }
    }
}
