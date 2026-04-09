using System.Globalization;
using System.Text;

namespace BLL.Services.Colors
{
    public static class BaseColorNormalizer
    {
        private static readonly string[] AllowedBaseColors =
        [
            "червоний",
            "рожевий",
            "білий",
            "жовтий",
            "помаранчевий",
            "фіолетовий",
            "синій",
            "блакитний",
            "зелений",
            "бежевий"
        ];

        private static readonly Dictionary<string, string> AliasToBaseColor = new(StringComparer.OrdinalIgnoreCase)
        {
            ["червоний"] = "червоний",
            ["red"] = "червоний",
            ["бордовий"] = "червоний",
            ["бургунді"] = "червоний",
            ["рубіновий"] = "червоний",
            ["малиновий"] = "червоний",
            ["кораловий"] = "червоний",

            ["рожевий"] = "рожевий",
            ["pink"] = "рожевий",
            ["пудровий"] = "рожевий",
            ["пудрово-рожевий"] = "рожевий",
            ["пастельно-рожевий"] = "рожевий",
            ["фуксія"] = "рожевий",
            ["персиковий"] = "рожевий",
            ["лососевий"] = "рожевий",

            ["білий"] = "білий",
            ["white"] = "білий",
            ["айворі"] = "білий",
            ["ivory"] = "білий",
            ["кремовий"] = "білий",
            ["молочний"] = "білий",

            ["жовтий"] = "жовтий",
            ["yellow"] = "жовтий",
            ["золотий"] = "жовтий",
            ["лимонний"] = "жовтий",

            ["помаранчевий"] = "помаранчевий",
            ["orange"] = "помаранчевий",
            ["абрикосовий"] = "помаранчевий",
            ["теракотовий"] = "помаранчевий",
            ["мідний"] = "помаранчевий",

            ["фіолетовий"] = "фіолетовий",
            ["purple"] = "фіолетовий",
            ["violet"] = "фіолетовий",
            ["лавандовий"] = "фіолетовий",
            ["ліловий"] = "фіолетовий",
            ["сиреневий"] = "фіолетовий",

            ["синій"] = "синій",
            ["blue"] = "синій",
            ["темно-синій"] = "синій",
            ["navy"] = "синій",
            ["індиго"] = "синій",

            ["блакитний"] = "блакитний",
            ["light blue"] = "блакитний",
            ["sky blue"] = "блакитний",
            ["небесний"] = "блакитний",
            ["бірюзовий"] = "блакитний",
            ["м'ятний"] = "блакитний",

            ["зелений"] = "зелений",
            ["green"] = "зелений",
            ["оливковий"] = "зелений",
            ["смарагдовий"] = "зелений",
            ["салатовий"] = "зелений",

            ["бежевий"] = "бежевий",
            ["beige"] = "бежевий",
            ["пісочний"] = "бежевий",
            ["карамельний"] = "бежевий",
            ["нюдовий"] = "бежевий"
        };

        public static IReadOnlyList<string> GetAllowedBaseColors() => AllowedBaseColors;

        public static string Normalize(string colorName)
        {
            if (string.IsNullOrWhiteSpace(colorName))
            {
                return null;
            }

            var prepared = Prepare(colorName);

            if (AliasToBaseColor.TryGetValue(prepared, out var mapped))
            {
                return mapped;
            }

            foreach (var allowedBaseColor in AllowedBaseColors)
            {
                if (prepared.Contains(allowedBaseColor, StringComparison.OrdinalIgnoreCase))
                {
                    return allowedBaseColor;
                }
            }

            return null;
        }

        public static List<string> NormalizeMany(IEnumerable<string> colorNames)
        {
            if (colorNames == null)
            {
                return [];
            }

            return colorNames
                .Select(Normalize)
                .Where(color => !string.IsNullOrWhiteSpace(color))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string Prepare(string value)
        {
            var normalized = value.Trim().ToLowerInvariant().Replace('_', ' ');
            return normalized;
        }
    }
}
