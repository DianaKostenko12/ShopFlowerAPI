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
        internal static string BuildStylePrompt(GenerateBouquetDescriptor bouquet, IReadOnlyCollection<string> allowedShades)
        {
            string bouquetColors = bouquet.Color is { Count: > 0 }
                ? string.Join(", ", bouquet.Color)
                : "РЅРµ РІРєР°Р·Р°РЅРѕ";
            string allowedBaseColors = string.Join(", ", BaseColorNormalizer.GetAllowedBaseColors());
            string allowedShadesText = string.Join(", ", allowedShades);
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
                - shade should be a short Ukrainian descriptor, preferably one of: {{allowedShadesText}}
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
                - For flowers, return general flower categories that best fit the requested style.
                - Category should describe the flower type/category, not a bouquet role, mood, or visual adjective.
                - Example flower categories: "троянда", "півонія", "тюльпан", "лілія", "хризантема", "гортензія", "еустома", "гіпсофіла".
                - These are only examples; you may return any other flower category if it matches the style better.
                - Use only exact canonical category names in singular nominative form.
                - Do not use plural, synonyms, or descriptive variations.
                - Examples: "троянда", not "троянди"; "лілія", not "лілії"; "гіпсофіла", not "гіпсофіли"; "хризантема", not
                "хризантеми".
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
                    "Round bouquet with a soft dome shape, viewed from above at a slight angle. Naturally rounded silhouette with organic flower placement вЂ” not perfectly symmetrical. Blooms sit at slightly different heights, creating a gentle, pillowy dome.",
                "асиметрична" =>
                    "Asymmetrical bouquet with one tall, lush dominant side and one low, restrained side. The dominant side rises noticeably higher with flowers leaning outward. The low side stays close to the base. Overall off-center, dynamic silhouette.",
                "подовжена" =>
                    "Elongated bouquet viewed from above at a slight angle. The flower arrangement is horizontally elongated вЂ” distinctly wider than it is tall, roughly 2:1 width-to-height ratio, with a soft oval shape. The bouquet is wrapped in decorative paper that gathers and tapers at the bottom, clearly showing this is a wrapped bouquet and not just a flat arrangement. The wrapping paper cradles the flowers and its gathered end is visible below the flower mass. Lush and compact flower dome on top, with the paper wrapper forming an elegant elongated frame around it.",
                _ =>
                    "Round bouquet with a soft dome shape, viewed from above at a slight angle. Naturally rounded silhouette with organic flower placement вЂ” not perfectly symmetrical. Blooms sit at slightly different heights, creating a gentle, pillowy dome."
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
                    - Focal flowers are clustered near the center, slightly offset from one another вЂ” never in a perfect grid or ring.
                    - Semi and filler flowers nestle organically around the focals, filling space naturally with slight overlaps.
                    - Flowers tilt gently inward toward the center (2-10В°), creating a soft, rounded dome profile.
                    - Small natural gaps between blooms where greenery and tiny filler flowers peek through.
                    - Overall silhouette is round but with the gentle irregularity of a hand-tied arrangement.
                    - No strict concentric rings вЂ” placement should feel organic, as if arranged by a skilled florist.
                    """,
                "асиметрична" =>
                    """
                    - Smallest flower heads are positioned near the center, largest heads on the outer edges.
                    - Flower roles do not determine position; head size matters more.
                    - The entire composition shifts to one side, creating a deliberate off-center look.
                    - On the dominant side, flowers rise progressively higher and lean outward at steeper angles.
                    - On the low side, flowers stay close to the base with minimal tilt.
                    """,
                "подовжена" =>
                    """
                    - The flower area forms a horizontally elongated oval, wider than it is tall.
                    - Focal flowers are spaced along the central horizontal axis of the bouquet.
                    - Semi and filler flowers fill the space between and around focal flowers, following the elongated contour.
                    - Flowers near the left and right edges are smaller and tilt slightly outward along the horizontal axis.
                    - The widest point of the flower mass is at the center, narrowing gently toward the left and right ends.
                    - Below the flowers, the wrapping paper gathers into a tapered point, making it clear this is a bouquet, not a flat shape.
                    - Greenery and filler peek naturally between blooms, softening the edges.
                    - The overall shape should clearly read as an elongated wrapped bouquet вЂ” not a boat, leaf, or standalone flat arrangement.
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

