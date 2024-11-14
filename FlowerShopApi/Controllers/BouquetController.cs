using AutoMapper;
using BLL.Services.Bouquets;
using BLL.Services.Bouquets.Descriptors;
using DAL.Filters;
using FlowerShopApi.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("bouquet")]
    public class BouquetController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBouquetService _bouquetService;

        public BouquetController(IBouquetService bouquetService,IHttpContextAccessor httpContextAccessor)
        {
            _bouquetService = bouquetService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddBouquetAsync([FromBody] CreateBouquetDescriptor descriptor)
        {
            try
            {
                int userId = _httpContextAccessor.HttpContext.User.GetUserId();
                await _bouquetService.AddBouquetAsync(descriptor, userId);
                return Ok(new { Message = "Bouquet added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBouquetAsync(int bouquetId)
        {
            try
            {
                await _bouquetService.DeleteBouquetAsync(bouquetId);
                return Ok(new { Message = "Bouquet deleted successfully." });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBouquetByUserIdAsync(int userId)
        {
            try
            {
                var bouquets = await _bouquetService.GetBouquetByUserIdAsync(userId);
                return Ok(bouquets);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetBouquetsByFilterAsync([FromBody] BouquetFilterView view)
        {
            try
            {
                var bouquets = await _bouquetService.GetBouquetsByFilterAsync(view);
                return Ok(bouquets);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBouquetAsync([FromBody] UpdateBouquetDescriptor descriptor)
        {
            try
            {
                await _bouquetService.UpdateBouquetAsync(descriptor);
                return Ok(new { Message = "Bouquet updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
