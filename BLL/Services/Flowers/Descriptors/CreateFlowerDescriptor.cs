using Microsoft.AspNetCore.Http;

namespace BLL.Services.Flowers.Descriptors
{
    public class CreateFlowerDescriptor
    {
        public string FlowerName { get; set; }
        public int FlowerCount { get; set; }
        public IFormFile Photo { get; set; }
        public float FlowerCost { get; set; }
    }
}
