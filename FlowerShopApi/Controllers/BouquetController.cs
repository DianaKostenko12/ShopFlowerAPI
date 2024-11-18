using AutoMapper;
using BLL.Services.Bouquets;
using BLL.Services.Bouquets.Descriptors;
using DAL.Filters;
using FlowerShopApi.Common.Extensions;
using FlowerShopApi.DTOs.Bouquets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("bouquet")]
    public class BouquetController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBouquetService _bouquetService;
        private readonly IMapper _mapper;

        public BouquetController(IBouquetService bouquetService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _bouquetService = bouquetService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        [HttpPost, Authorize(Roles = "Admin, Customer")]
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

        [HttpDelete, Authorize]
        public async Task<IActionResult> DeleteBouquetAsync(int bouquetId)
        {
            int userId = _httpContextAccessor.HttpContext.User.GetUserId();
            await _bouquetService.DeleteBouquetAsync(bouquetId, userId);

            return Ok(new { Message = "Bouquet deleted successfully." });
        }

        [HttpGet("{userId}"), Authorize]
        public async Task<IActionResult> GetBouquetsByUserIdAsync(int userId)
        {
            try
            {
                var bouquets = await _bouquetService.GetBouquetsByUserIdAsync(userId);
                var bouquetsDto = _mapper.Map<List<GetBouquetResponse>>(bouquets);
                return Ok(bouquetsDto);
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
                var bouquetsDto = _mapper.Map<List<GetBouquetResponse>>(bouquets);
                return Ok(bouquetsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        //[HttpGet]
        //[HttpPut]
        //public async Task<IActionResult> UpdateBouquetAsync([FromBody] UpdateBouquetDescriptor descriptor)
        //{
        //    try
        //    {
        //        await _bouquetService.UpdateBouquetAsync(descriptor);
        //        return Ok(new { Message = "Bouquet updated successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}
    } 
}
