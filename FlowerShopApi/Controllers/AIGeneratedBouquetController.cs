using BLL.Services.BouquetGeneration;
using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.OpenAi.Dto;
using FlowerShopApi.DTOs.AIGeneratedBouquets;
using FlowerShopApi.Services.AIGeneratedBouquets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("AIGeneratedBouquet")]
    public class AIGeneratedBouquetController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBouquetGenerationService _bouquetGenerationService;
        private readonly IAIGeneratedBouquetResponseService _aiGeneratedBouquetResponseService;

        public AIGeneratedBouquetController(
            IHttpContextAccessor httpContextAccessor,
            IBouquetGenerationService bouquetGenerationService,
            IAIGeneratedBouquetResponseService aiGeneratedBouquetResponseService)
        {
            _httpContextAccessor = httpContextAccessor;
            _bouquetGenerationService = bouquetGenerationService;
            _aiGeneratedBouquetResponseService = aiGeneratedBouquetResponseService;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> GenerateAIBouquetAsync([FromBody] GenerateAIBouquetRequest request)
        {
            try
            {
                var requestedColors = request.Color?
                    .Select(color => new ColorPreference(color.BaseColor, color.Shade))
                    .ToList()
                    ?? [];

                GenerateBouquetDescriptor descriptor = new(requestedColors, request.Budget, request.Style, request.Shape, request.AdditionalComment);
                var generatedAIBouquet = await _bouquetGenerationService.GenerateBouquetAsync(descriptor);
                var response = _aiGeneratedBouquetResponseService.BuildResponse(generatedAIBouquet);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
    }
}
