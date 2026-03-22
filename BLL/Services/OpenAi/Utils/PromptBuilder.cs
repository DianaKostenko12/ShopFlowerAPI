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
            return $$"""
                You are a florist assistant.

                Style: {{bouquet.Style}}
                Shape: {{bouquet.Shape}}
                Color preference: {{bouquet.Color}}

                Task:
                Generate bouquet composition details.

                Rules:
                - Generate a creative bouquet name (3–5 words).
                - Palette must include harmonious primary and accent colors.
                - Use only generic flower categories (e.g. Rose, Peony, Tulip, Eustoma, Greenery).
                - Focal: max 2 categories.
                - Semi: max 2 categories.
                - Filler: max 2 categories.
                - Greenery: max 1 category.
                - Min/Max should be realistic (e.g. 1–5 stems depending on role).
                - Wrapping paper should match palette and style.
                - Do NOT include Style or Shape in response.

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

                Bouquet composition:
                {BuildFlowerList(bouquetDetails.FlowerComposition)}

                Arrangement rules:
                - Focal flowers are placed in the center and visually dominate.
                - Semi flowers surround the focal flowers.
                - Filler flowers fill gaps and add softness.
                - Greenery frames the bouquet and adds volume.

                Wrapping:
                {BuildWrapping(bouquetDetails.WrappingPaper)}

                Style and rendering:
                - Natural florist style
                - Balanced and harmonious composition
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
    }
}
