using DAL.Models.Flowers;

namespace DAL.Models
{
    public class WrappingPaper
    {
        public int WrappingPaperId { get; set; }
        public WrappingPaperType Type { get; set; }
        public int ColorId { get; set; }
        public Color Color { get; set; }
        public WrappingPaperPattern Pattern { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
