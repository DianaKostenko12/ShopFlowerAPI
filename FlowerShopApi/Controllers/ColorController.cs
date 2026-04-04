using AutoMapper;
using BLL.Services.Colors;
using DAL.Models.Flowers;
using FlowerShopApi.DTOs.Colors;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("color")]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _colorService;
        private readonly IMapper _mapper;

        public ColorController(IColorService colorService, IMapper mapper)
        {
            _colorService = colorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllColors()
        {
            var colors = await _colorService.GetColorsAsync();
            return Ok(_mapper.Map<List<ColorResponse>>(colors));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetColorById(int id)
        {
            var color = await _colorService.GetColorByIdAsync(id);
            if (color == null)
            {
                return NotFound($"Color with ID {id} not found.");
            }

            return Ok(_mapper.Map<ColorResponse>(color));
        }

        [HttpPost]
        public async Task<IActionResult> AddColor([FromBody] CreateColorRequest request)
        {
            var color = _mapper.Map<Color>(request);
            await _colorService.AddColorAsync(color);

            return Ok("Successfully created");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            await _colorService.DeleteColorAsync(id);
            return Ok("Color successfully deleted.");
        }
    }
}
