using DAL.Models;

namespace FlowerShopApi.DTOs.WrappingPapers
{
    public class WrappingPaperResponse
    {
        public int WrappingPaperId { get; set; }
        public WrappingPaperType Type { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorShade { get; set; }
        public WrappingPaperPattern Pattern { get; set; }
    }
}
