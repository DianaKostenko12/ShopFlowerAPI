using AutoMapper;
using BLL.Services.Bouquets;
using BLL.Services.Bouquets.Descriptors;
using DAL.Filters;
using FlowerShopApi.Common.Extensions;
using FlowerShopApi.DTOs.Bouquets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("bouquet")]
    public class BouquetController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBouquetService _bouquetService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public BouquetController(IBouquetService bouquetService, IHttpContextAccessor httpContextAccessor, IMapper mapper, IWebHostEnvironment environment)
        {
            _bouquetService = bouquetService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddBouquetAsync([FromForm] CreateBouquetRequest request)
        {
            try
            {
                int? userId = _httpContextAccessor.HttpContext.User.GetUserId();
                var descriptor = _mapper.Map<CreateBouquetDescriptor>(request);
                await _bouquetService.AddBouquetAsync(descriptor, userId.Value);
                return Ok(new { Message = "Bouquet added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("ai"), Authorize]
        public async Task<IActionResult> AddAIBouquetAsync([FromBody] CreateAIBouquetRequest request)
        {
            try
            {
                int? userId = _httpContextAccessor.HttpContext.User.GetUserId();
                var descriptor = _mapper.Map<CreateBouquetDescriptor>(request);
                await _bouquetService.AddBouquetAsync(descriptor, userId.Value);
                return Ok(new { Message = "AI bouquet added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete, Authorize]
        public async Task<IActionResult> DeleteBouquetAsync(int bouquetId)
        {
            int? userId = _httpContextAccessor.HttpContext.User.GetUserId();
            await _bouquetService.DeleteBouquetAsync(bouquetId, userId.Value);

            return Ok(new { Message = "Bouquet deleted successfully." });
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetBouquetsByUserIdAsync()
        {
            int? userId = _httpContextAccessor.HttpContext.User.GetUserId();
            try
            {
                var bouquets = await _bouquetService.GetBouquetsByUserIdAsync(userId.Value);
                var bouquetsDto = _mapper.Map<List<GetBouquetResponse>>(bouquets);
                return Ok(bouquetsDto);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetBouquetImageAsync(int id)
        {
            var bouquet = await _bouquetService.GetBouquetByIdAsync(id);
            if (bouquet == null)
            {
                return NotFound(new { Message = $"Bouquet with ID {id} was not found." });
            }

            if (bouquet.PhotoBytes != null && bouquet.PhotoBytes.Length > 0)
            {
                return File(bouquet.PhotoBytes, bouquet.PhotoContentType ?? "image/png");
            }

            if (!string.IsNullOrWhiteSpace(bouquet.PhotoFileName))
            {
                var filePath = Path.Combine(_environment.ContentRootPath, "UploadedFiles", bouquet.PhotoFileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { Message = "Bouquet image file was not found." });
                }

                var extension = Path.GetExtension(bouquet.PhotoFileName).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, contentType);
            }

            return NotFound(new { Message = "Bouquet image was not found." });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetBouquetsByFilterAsync([FromBody] BouquetFilterView view)
        {
            try
            {
                int? userId = _httpContextAccessor.HttpContext.User.GetUserId();
                var bouquets = await _bouquetService.GetBouquetsByFilterAsync(view, userId);
                var bouquetsDto = _mapper.Map<List<GetBouquetResponse>>(bouquets);
                return Ok(bouquetsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
