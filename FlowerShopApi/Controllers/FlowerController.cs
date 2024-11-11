using AutoMapper;
using BLL.Services.Auth;
using BLL.Services.Flowers;
using BLL.Services.Flowers.Descriptors;
using DAL.Models;
using FlowerShopApi.DTOs.Flowers;
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
            if (flowersData == null || !flowersData.Any())
            {
                return NotFound("No flowers found.");
            }
            var flowers = _mapper.Map<List<FlowerRequest>>(flowersData);
            return Ok(flowers);
        }

        [HttpGet("{flowerId}")]
        public async Task<IActionResult> GetFlowerById(int flowerId)
        {
            var flowerData = await _flowerService.GetFlowerByIdAsync(flowerId);
            if (flowerData == null)
            {
                return NotFound($"Flower with ID {flowerId} not found.");
            }
            var flower = _mapper.Map<FlowerRequest>(flowerData);
            return Ok(flower);
        }

        [HttpPost]
        public async Task<IActionResult> AddFlower([FromBody] CreateFlower flowerDto)
        {
            if (flowerDto == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid flower data.");
            }
            var descriptor = _mapper.Map<CreateFlowerDescriptor>(flowerDto);
            await _flowerService.AddFlowerAsync(descriptor);
            return Ok("Successfully created");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFlower([FromBody] FlowerRequest flowerDto)
        {
            if (flowerDto == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid flower data.");
            }

            var descriptor = _mapper.Map<UpdateFlowerDescriptor>(flowerDto);
            await _flowerService.UpdateFlowerAsync(descriptor);
            return Ok("Flower successfully updated.");
        }

        [HttpDelete("{flowerId}")]
        public async Task<IActionResult> DeleteFlower(int flowerId)
        {
            await _flowerService.DeleteFlowerAsync(flowerId);
            return Ok("Flower successfully deleted.");
        }
    }
}
