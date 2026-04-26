using DAL.Data.UnitOfWork;
using DAL.Models.Flowers;

namespace BLL.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            await _uow.CategoryRepository.AddAsync(category);
            await _uow.CompleteAsync();

            return category;
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _uow.CategoryRepository.FindAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} was not found.");
            }

            await _uow.CategoryRepository.RemoveAsync(category);
            await _uow.CompleteAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _uow.CategoryRepository.FindAllAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await _uow.CategoryRepository.FindAsync(categoryId);
        }
    }
}
