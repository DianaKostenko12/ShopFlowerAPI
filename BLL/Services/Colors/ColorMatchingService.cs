using BLL.Services.OpenAi.Dto;
using DAL.Models.Flowers;

namespace BLL.Services.Colors
{
    public class ColorMatchingService : IColorMatchingService
    {
        public double? ResolveColorScore(
            Color flowerColor,
            IEnumerable<ColorPreference> requestedColors,
            double baseMatchScore)
        {
            var flowerBaseColor = BaseColorNormalizer.Normalize(flowerColor?.ColorName);
            if (string.IsNullOrWhiteSpace(flowerBaseColor))
            {
                return null;
            }

            foreach (var requestedColor in requestedColors)
            {
                var requestedBaseColor = BaseColorNormalizer.Normalize(requestedColor.BaseColor);
                if (!flowerBaseColor.Equals(requestedBaseColor, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (HasMatchingShade(flowerColor?.Shade, requestedColor.Shade))
                {
                    return baseMatchScore + 0.2;
                }

                return baseMatchScore;
            }

            return null;
        }

        public bool MatchesWrappingColor(Color wrappingColor, IEnumerable<ColorPreference> requestedColors)
        {
            var normalizedWrappingBaseColor = BaseColorNormalizer.Normalize(wrappingColor?.ColorName);
            if (string.IsNullOrWhiteSpace(normalizedWrappingBaseColor))
            {
                return false;
            }

            foreach (var requestedColor in requestedColors)
            {
                var requestedBaseColor = BaseColorNormalizer.Normalize(requestedColor.BaseColor);
                if (!normalizedWrappingBaseColor.Equals(requestedBaseColor, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (HasMatchingShade(wrappingColor?.Shade, requestedColor.Shade))
                {
                    return true;
                }

                return true;
            }

            return false;
        }

        private static bool HasMatchingShade(string flowerShade, string requestedShade)
        {
            if (string.IsNullOrWhiteSpace(flowerShade) || string.IsNullOrWhiteSpace(requestedShade))
            {
                return false;
            }

            return flowerShade.Trim().Equals(requestedShade.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
