using AutoMapper;
using BLL.Services.Categories;
using DAL.Models.Flowers;
using FlowerShopApi.DTOs.Categories;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return Ok(_mapper.Map<List<CategoryResponse>>(categories));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            return Ok(_mapper.Map<CategoryResponse>(category));
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryRequest request)
        {
            var category = _mapper.Map<Category>(request);
            var createdCategory = await _categoryService.AddCategoryAsync(category);

            return Ok(_mapper.Map<CategoryResponse>(createdCategory));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok("Category successfully deleted.");
        }
    }
}
