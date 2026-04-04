using DAL.Data.UnitOfWork;
using DAL.Models.Flowers;

namespace BLL.Services.Colors
{
    public class ColorService : IColorService
    {
        private readonly IUnitOfWork _uow;

        public ColorService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task AddColorAsync(Color color)
        {
            ArgumentNullException.ThrowIfNull(color);

            await _uow.ColorRepository.AddAsync(color);
            await _uow.CompleteAsync();
        }

        public async Task DeleteColorAsync(int colorId)
        {
            var color = await _uow.ColorRepository.FindAsync(colorId);
            if (color == null)
            {
                throw new KeyNotFoundException($"Color with ID {colorId} was not found.");
            }

            await _uow.ColorRepository.RemoveAsync(color);
            await _uow.CompleteAsync();
        }

        public async Task<IEnumerable<Color>> GetColorsAsync()
        {
            return await _uow.ColorRepository.FindAllAsync();
        }

        public async Task<Color> GetColorByIdAsync(int colorId)
        {
            return await _uow.ColorRepository.FindAsync(colorId);
        }
    }
}
