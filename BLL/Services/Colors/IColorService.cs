using DAL.Models.Flowers;

namespace BLL.Services.Colors
{
    public interface IColorService
    {
        Task<IEnumerable<Color>> GetColorsAsync();
        Task<Color> GetColorByIdAsync(int colorId);
        Task<Color> AddColorAsync(Color color);
        Task DeleteColorAsync(int colorId);
    }
}
