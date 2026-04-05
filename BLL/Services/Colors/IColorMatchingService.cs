using BLL.Services.OpenAi.Dto;
using DAL.Models.Flowers;

namespace BLL.Services.Colors
{
    public interface IColorMatchingService
    {
        double? ResolveColorScore(Color flowerColor, IEnumerable<ColorPreference> requestedColors, double baseMatchScore);
        bool MatchesWrappingColor(Color wrappingColor, IEnumerable<ColorPreference> requestedColors);
    }
}
