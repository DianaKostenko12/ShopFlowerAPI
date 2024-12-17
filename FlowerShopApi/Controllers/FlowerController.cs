using AutoMapper;
using BLL.Services.Auth;
using BLL.Services.Flowers;
using BLL.Services.Flowers.Descriptors;
using DAL.Models;
using FlowerShopApi.DTOs.Flowers;
using FlowerShopApi.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("flower")]
    public class FlowerController : ControllerBase
    {
        private readonly IFlowerService _flowerService;
        private readonly IMapper _mapper;

        public FlowerController(IFlowerService flowerService, IMapper mapper)
        {
            _flowerService = flowerService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFlowers()
        {
            var flowersData = await _flowerService.GetFlowersAsync();
   
            var flowers = _mapper.Map<List<FlowerResponse>>(flowersData);

            for (int i = 0; i < flowers.Count; i++)
            {
                flowers[i].ImgUrl = $"{Request.Scheme}://{Request.Host}/uploads/{flowers[i].ImgUrl}";
            }

            return Ok(flowers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlowerById(int flowerId)
        {
            var flowerData = await _flowerService.GetFlowerByIdAsync(flowerId);
            if (flowerData == null)
            {
                return NotFound($"Flower with ID {flowerId} not found.");
            }

            var flower = _mapper.Map<FlowerResponse>(flowerData);
            return Ok(flower);
        }

        [HttpPost]
        public async Task<IActionResult> AddFlower([FromForm] CreateFlower flowerDto)
        {
            var descriptor = _mapper.Map<CreateFlowerDescriptor>(flowerDto);
            await _flowerService.AddFlowerAsync(descriptor);

            return Ok("Successfully created");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFlower([FromBody] FlowerResponse flowerDto)
        {
            var descriptor = _mapper.Map<UpdateFlowerDescriptor>(flowerDto);
            await _flowerService.UpdateFlowerAsync(descriptor);

            return Ok("Flower successfully updated.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFlower(int flowerId)
        {
            await _flowerService.DeleteFlowerAsync(flowerId);
            return Ok("Flower successfully deleted.");
        }

       
                
    }
}
