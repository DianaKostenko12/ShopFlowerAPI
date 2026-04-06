using AutoMapper;
using BLL.Services.WrappingPapers;
using DAL.Models;
using FlowerShopApi.DTOs.WrappingPapers;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("wrapping-paper")]
    public class WrappingPaperController : ControllerBase
    {
        private readonly IWrappingPaperService _wrappingPaperService;
        private readonly IMapper _mapper;

        public WrappingPaperController(IWrappingPaperService wrappingPaperService, IMapper mapper)
        {
            _wrappingPaperService = wrappingPaperService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWrappingPapers()
        {
            var wrappingPapers = await _wrappingPaperService.GetWrappingPapersAsync();
            return Ok(_mapper.Map<List<WrappingPaperResponse>>(wrappingPapers));
        }

        [HttpPost]
        public async Task<IActionResult> AddWrappingPaper([FromBody] CreateWrappingPaperRequest request)
        {
            var wrappingPaper = _mapper.Map<WrappingPaper>(request);
            await _wrappingPaperService.AddWrappingPaperAsync(wrappingPaper);

            return Ok("Successfully created");
        }
    }
}
