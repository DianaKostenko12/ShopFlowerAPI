using DAL.Models.Flowers;

namespace BLL.Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task AddCategoryAsync(Category category);
        Task DeleteCategoryAsync(int categoryId);
    }
}
