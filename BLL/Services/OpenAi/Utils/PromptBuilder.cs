using BLL.Services.BouquetGeneration.BouquetPlanner;
using BLL.Services.BouquetGeneration.BouquetPlanner.Dto;
using BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition.Dto;
using BLL.Services.BouquetGeneration.Descriptors;
using DAL.Models;
using System.Text;

namespace BLL.Services.OpenAi.Utils
{
    internal static class PromptBuilder
    {
        internal static string BuildStylePrompt(GenerateBouquetDescriptor bouquet)
        {
            string bouquetColors = string.Join(",", bouquet.Color);

            return $$"""
                You are a florist assistant.

                Style: {{bouquet.Style}}
                Shape: {{bouquet.Shape}}
                Color preference: {{bouquetColors}}
                Budget: {{bouquet.Budget}}

                Task:
                Generate bouquet composition details.

                STRICT RULES:
                - Return ONLY valid JSON. No explanations, no markdown, no ``` blocks.
                - Do not include any text before or after JSON.
                - All fields are required.
                - Use camelCase exactly as in schema.
                - Numbers must be integers.

                COLOR RULES:
                - Use ONLY simple base colors (one word).
                - Do NOT use adjectives like "light", "dark", "soft", "pale", "sky", etc.
                - Examples of valid colors: red, pink, blue, white, yellow, green, purple, beige.

                BUDGET RULES:
                - The bouquet must realistically fit within the given budget.
                - Low budget → fewer flowers, simpler composition.
                - Medium budget → balanced composition.
                - High budget → richer composition with more focal flowers.
                - Do NOT generate excessive quantities if budget is limited.
                - Adjust min/max values according to budget.

                Bouquet rules:
                - Generate a creative bouquet name (3–5 words).
                - Palette must include 2–3 primary and 1–2 accent colors.
                - Use only generic flower categories (Rose, Peony, Tulip, Eustoma, Hydrangea, Ranunculus, Gypsophila, Greenery).
                - Focal: max 2 categories.
                - Semi: max 2 categories.
                - Filler: max 2 categories.
                - Greenery: exactly 1 category.
                - Wrapping paper must match palette and style.

                Return STRICT JSON in this format:
                {
                  "bouquetName": "",
                  "palette": {
                    "primary": [],
                    "accent": []
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
                    "colors": [],
                    "patterns": []
                  }
                }
                """;
        }

        internal static string BuildImagePrompt(BouquetDetails bouquetDetails)
        {
            return $"""
                Create a realistic, high-quality photo of a professionally arranged flower bouquet.

                Bouquet shape:
                {ResolveShape(bouquetDetails.Shape)}

                Bouquet composition (Exact counts and items):
                {BuildFlowerList(bouquetDetails.FlowerComposition)}

                Arrangement rules:
                - Focal flowers are placed in the center and visually dominate.
                - Semi flowers surround the focal flowers.
                - Filler flowers fill gaps and add softness.
                {(HasGreenery(bouquetDetails.FlowerComposition) ? "- Greenery frames the bouquet and adds volume." : "- No greenery or leaves should be included.")}

                Wrapping:
                {BuildWrapping(bouquetDetails.WrappingPaper)}

                Style and rendering:
                - Natural florist style
                - Balanced and harmonious composition
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
                "round" => "Round bouquet with a balanced, symmetrical composition.",
                "elongated" => "Elongated vertical bouquet with elegant height.",
                "horizontal" => "Horizontal bouquet with wide, low-profile composition.",
                "asymmetrical" => "Asymmetrical bouquet with a natural, organic flow.",
                _ => "Round bouquet with a balanced, symmetrical composition."
            };
        }

        private static string BuildFlowerList(
            IEnumerable<FlowerComposition> flowers)
        {
            var sb = new StringBuilder();

            foreach (var f in flowers)
            {
                var role = ResolveRoleText(f.Role);
                sb.AppendLine($"- {f.Quantity} {f.flower.FlowerName.ToLowerInvariant()} as {role}");
            }

            return sb.ToString();
        }

        private static string ResolveRoleText(string role)
        {
            return role switch
            {
                RolesConstants.FocalCategory => "focal flowers",
                RolesConstants.SemiCategory => "semi flowers",
                RolesConstants.FillerCategory => "filler flowers",
                RolesConstants.GreeneryCategory => "greenery",
                _ => "flowers"
            };
        }

        private static string BuildWrapping(WrappingPaper wrapping)
        {
            if (wrapping == null)
            {
                return "No wrapping specified.";
            }

            return $"Wrapped in {wrapping.Pattern?.ToLowerInvariant() ?? "plain"} " +
                   $"{wrapping.Color?.ToLowerInvariant() ?? "neutral"} " +
                   $"{wrapping.Type?.ToLowerInvariant() ?? "paper"}.";
        }

        private static bool HasGreenery(IEnumerable<FlowerComposition> flowers)
        {
            if (flowers == null)
            {
                return false;
            }

            return flowers.Any(f => f.Role == RolesConstants.GreeneryCategory);
        }
    }
}
