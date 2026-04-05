using BLL.Services.BouquetGeneration.BouquetPlanner;
using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition.Dto;
using BLL.Services.BouquetGeneration.Descriptors;
using BLL.Services.Colors;
using DAL.Models;
using System.Text;

namespace BLL.Services.OpenAi.Utils
{
    internal static class PromptBuilder
    {
        private static readonly string[] AllowedShades =
        [
            "світлий",
            "темний",
            "теплий",
            "холодний",
            "пастельний",
            "ніжний",
            "яскравий",
            "насичений",
            "приглушений",
            "нейтральний"
        ];

        internal static string BuildStylePrompt(GenerateBouquetDescriptor bouquet)
        {
            string bouquetColors = bouquet.Color is { Count: > 0 }
                ? string.Join(", ", bouquet.Color)
                : "не вказано";
            string allowedBaseColors = string.Join(", ", BaseColorNormalizer.GetAllowedBaseColors());
            string allowedShades = string.Join(", ", AllowedShades);
            string allowedWrappingTypes = string.Join(", ", Enum.GetNames<WrappingPaperType>());
            string allowedWrappingPatterns = string.Join(", ", Enum.GetNames<WrappingPaperPattern>());

            return $$"""
                You are a florist assistant.

                User request:
                - Style: {{bouquet.Style}}
                - Shape: {{bouquet.Shape}}
                - Color preference: {{bouquetColors}}
                - Budget: {{bouquet.Budget}}
                - Additional comment: {{bouquet.AdditionalComment}}

                Return valid JSON only.
                No markdown, no comments, no extra text.
                All keys must stay exactly as in the schema.

                Language rules:
                - JSON keys: English only.
                - JSON values: Ukrainian only.
                - Exception: wrappingPaper.type and wrappingPaper.pattern must use exact enum values from the allowed lists.

                Color rules:
                - Every color must be an object with: baseColor, shade.
                - baseColor must be one of: {{allowedBaseColors}}
                - shade should be a short Ukrainian descriptor, preferably one of: {{allowedShades}}
                - Convert specific shades to baseColor + shade.
                - Example: бургунді -> червоний + темний
                - Example: айворі -> білий + теплий
                - Example: пудрово-рожевий -> рожевий + ніжний
                - Do not return specific database shade names.

                Wrapping rules:
                - wrappingPaper.type must be one of: {{allowedWrappingTypes}}
                - wrappingPaper.pattern must be one of: {{allowedWrappingPatterns}}
                - wrappingPaper.colors must follow the same baseColor + shade format.

                Bouquet rules:
                - bouquetName: 3-5 words, Ukrainian.
                - palette.primary: 2-3 colors.
                - palette.accent: 1-2 colors.
                - Focal/Semi/Filler: max 2 categories each.
                - Greenery: 1-3 categories.
                - Flower categories and greenery names: Ukrainian.
                - wrappingPaper colors should match the palette by meaning.
                - Quantities must be realistic for the budget.

                Before returning, verify:
                - JSON is valid.
                - All keys are unchanged.
                - Every color has both baseColor and shade.
                - baseColor values are from the allowed list.
                - wrappingPaper.type and wrappingPaper.pattern are from the allowed lists.

                Return this JSON structure only:
                {
                  "bouquetName": "",
                  "palette": {
                    "primary": [
                      {
                        "baseColor": "",
                        "shade": ""
                      }
                    ],
                    "accent": [
                      {
                        "baseColor": "",
                        "shade": ""
                      }
                    ]
                  },
                  "roles": {
                    "focal": {
                      "categories": [],
                      "min": 0,
                      "max": 0
                    },
                    "semi": {
                      "categories": [],
                      "min": 0,
                      "max": 0
                    },
                    "filler": {
                      "categories": [],
                      "min": 0,
                      "max": 0
                    },
                    "greenery": {
                      "categories": [],
                      "min": 0,
                      "max": 0
                    }
                  },
                  "wrappingPaper": {
                    "colors": [
                      {
                        "baseColor": "",
                        "shade": ""
                      }
                    ],
                    "type": "",
                    "pattern": ""
                  }
                }
                """;
        }

        internal static string BuildImagePrompt(BouquetDetails bouquetDetails)
        {
            var hasGreenery = HasGreenery(bouquetDetails.FlowerComposition);

            return $"""
                Create a realistic, high-quality photo of a professionally arranged flower bouquet.

                Bouquet shape:
                {ResolveShape(bouquetDetails.Shape)}

                Bouquet composition (Exact counts and items):
                {BuildFlowerList(bouquetDetails.FlowerComposition)}

                Arrangement rules:
                {BuildArrangementRules(bouquetDetails.Shape, hasGreenery)}

                Wrapping:
                {BuildWrapping(bouquetDetails.WrappingPaper)}

                Style and rendering:
                - Natural florist style
                - Each flower must be a distinct, individual object (to help with counting)
                - All flowers clearly visible and identifiable
                - Soft natural daylight
                - Neutral background
                - Photorealistic, studio-quality image
            """;
        }

        private static string ResolveShape(string shape)
        {
            return shape?.ToLowerInvariant() switch
            {
                "кругла" =>
                    "Round dome-shaped bouquet viewed from above as concentric circles. Perfectly symmetrical in all directions. Flat top, compact and full.",
                "подовжена" =>
                    "Wide crescent-shaped bouquet, much wider than it is deep. Front-facing half-moon silhouette with gently curving edges. Flowers at the tips angle outward, center flowers point straight up.",
                "асиметрична" =>
                    "Asymmetrical bouquet with one tall, lush dominant side and one low, restrained side. The dominant side rises noticeably higher with flowers leaning outward. The low side stays close to the base. Overall off-center, dynamic silhouette.",
                _ =>
                    "Round dome-shaped bouquet viewed from above as concentric circles. Perfectly symmetrical in all directions. Flat top, compact and full."
            };
        }

        private static string BuildArrangementRules(string shape, bool hasGreenery)
        {
            var greeneryRule = hasGreenery
                ? "- Greenery is evenly interspersed among other flowers throughout the arrangement."
                : "- No greenery or leaves should be included.";

            var shapeRules = shape?.ToLowerInvariant() switch
            {
                "кругла" =>
                    """
                    - One focal flower sits at the exact center of the bouquet.
                    - Additional focal flowers form a tight first ring around the center.
                    - Semi, filler, and greenery flowers are evenly mixed in alternating concentric rings radiating outward.
                    - Each ring has uniform spacing between flowers.
                    - All flowers point straight up with no tilt, creating a flat dome profile.
                    """,
                "подовжена" =>
                    """
                    - Focal flowers form the central spine running along the crescent arc.
                    - Other flowers fill outward in elliptical layers, with width growing much faster than depth.
                    - The bouquet spreads wide left-to-right but stays shallow front-to-back.
                    - Flowers near the crescent tips tilt outward up to 30 degrees.
                    - Center flowers stand upright.
                    """,
                "асиметрична" =>
                    """
                    - Smallest flower heads are positioned near the center, largest heads on the outer edges.
                    - Flower roles do not determine position; head size matters more.
                    - The entire composition shifts to one side, creating a deliberate off-center look.
                    - On the dominant side, flowers rise progressively higher and lean outward at steeper angles.
                    - On the low side, flowers stay close to the base with minimal tilt.
                    """,
                _ =>
                    """
                    - Focal flowers are placed in the center and visually dominate.
                    - Semi flowers surround the focal flowers.
                    - Filler flowers fill gaps and add softness.
                    """
            };

            return shapeRules.TrimEnd() + "\n" + greeneryRule;
        }

        private static string BuildFlowerList(IEnumerable<FlowerComposition> flowers)
        {
            var sb = new StringBuilder();

            foreach (var f in flowers)
            {
                var role = ResolveRoleText(f.Role);
                sb.AppendLine($"- {f.Quantity} pieces of {f.flower.FlowerName.ToLowerInvariant()} as {role} in color {f.flower.Color?.ColorName}");
            }

            return sb.ToString();
        }

        private static string ResolveRoleText(FlowerRole role)
        {
            return role switch
            {
                FlowerRole.Focal => "focal flowers",
                FlowerRole.Semi => "semi flowers",
                FlowerRole.Filler => "filler flowers",
                FlowerRole.Greenery => "greenery",
                _ => "flowers"
            };
        }

        private static string BuildWrapping(WrappingPaper wrapping)
        {
            if (wrapping == null)
            {
                return "No wrapping specified.";
            }

            var patternText = wrapping.Pattern switch
            {
                WrappingPaperPattern.Plain => "plain",
                WrappingPaperPattern.Lines => "lines",
                WrappingPaperPattern.Dots => "dots",
                WrappingPaperPattern.NoPattern => "without pattern",
                _ => "plain"
            };

            var typeText = wrapping.Type switch
            {
                WrappingPaperType.Paper => "paper",
                WrappingPaperType.Kraft => "kraft paper",
                WrappingPaperType.Film => "film wrap",
                WrappingPaperType.Mesh => "mesh wrap",
                WrappingPaperType.Fabric => "fabric wrap",
                _ => "paper"
            };

            return $"Wrapped in {patternText} {wrapping.Color?.ColorName?.ToLowerInvariant() ?? "neutral"} {typeText}.";
        }

        private static bool HasGreenery(IEnumerable<FlowerComposition> flowers)
        {
            if (flowers == null)
            {
                return false;
            }

            return flowers.Any(f => f.Role == FlowerRole.Greenery);
        }
    }
}
