using BLL.Services.BouquetGeneration;
using BLL.Services.BouquetGeneration.Descriptors;
using FlowerShopApi.DTOs.AIGeneratedBouquets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("AIGeneratedBouquet")]
    public class AIGeneratedBouquetController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBouquetGenerationService _bouquetGenerationService;
        public AIGeneratedBouquetController(IHttpContextAccessor httpContextAccessor, IBouquetGenerationService bouquetGenerationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _bouquetGenerationService = bouquetGenerationService;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> GenerateAIBouquetAsync([FromBody] GenerateAIBouquetRequest request)
        {
            try
            {
                GenerateBouquetDescriptor descriptor = new(request.Color, request.Budget, request.Style, request.Shape, request.AdditionalComment);
                var generatedAIBouquet = await _bouquetGenerationService.GenerateBouquetAsync(descriptor);
                return Ok(generatedAIBouquet);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
    }
}
