using DAL.Models;

namespace FlowerShopApi.DTOs.WrappingPapers
{
    public class CreateWrappingPaperRequest
    {
        public WrappingPaperType Type { get; set; }
        public int ColorId { get; set; }
        public WrappingPaperPattern Pattern { get; set; }
    }
}
