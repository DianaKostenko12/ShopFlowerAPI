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
                Згенерувати деталі композиції букета.

                STRICT OUTPUT FORMAT:

                Output MUST be valid JSON.
                Output MUST start with { and end with }.
                Do NOT include any text before or after JSON.
                Do NOT use markdown or ``` blocks.
                Do NOT add comments.
                Do NOT add trailing commas.
                All fields are required.

                CRITICAL STRUCTURE RULE:

                JSON keys MUST remain EXACTLY as defined in the schema.
                DO NOT translate keys.
                DO NOT rename keys.
                DO NOT remove keys.
                DO NOT add new keys.

                CRITICAL LANGUAGE RULE:

                JSON keys → English ONLY.
                JSON values → Ukrainian ONLY.
                ANY English word inside values is FORBIDDEN.
                If any value is not Ukrainian → response is INVALID.

                OUTPUT LANGUAGE RULES:

                bouquetName → Ukrainian only.
                palette colors → Ukrainian only.
                flower categories → Ukrainian only.
                wrappingPaper.colors → Ukrainian only.
                wrappingPaper.patterns → Ukrainian only.

                COLOR RULES:

                Use ONLY one-word base colors.
                Allowed colors ONLY:
                червоний, рожевий, синій, білий, жовтий, зелений, фіолетовий, бежевий
                Any other color is FORBIDDEN.
                No оттенки or descriptive adjectives.

                FLOWER CATEGORY RULES:
                Any flower and greenery categories are allowed in Ukrainian.
                Example flower categories: Троянда, Півонія, Тюльпан, Еустома, Гортензія, Ранункулюс, Гіпсофіла, Лілія, Орхідея, Хризантема, Айстра, Соняшник.
                Example greenery categories: Зелень, Евкаліпт, Аспарагус, Папороть, Рускус, Сальвія, Лаванда.
                For focal, semi, and filler – select categories logically according to the role.
                For greenery – you can specify 1–3 greenery categories depending on the budget.

                BUDGET RULES:

                Low budget → small numbers.
                Medium budget → balanced numbers.
                High budget → larger numbers.
                Numbers MUST be realistic.
                Avoid excessive quantities.

                Bouquet rules:

                bouquetName: 3–5 words, Ukrainian.
                palette: 2–3 primary, 1–2 accent.
                Focal: max 2 categories.
                Semi: max 2 categories.
                Filler: max 2 categories.
                Greenery: 1-3 categories of greenery.
                wrappingPaper.colors MUST be chosen from palette.
                wrappingPaper.patterns MUST be simple Ukrainian words:
                (однотонний, лінії, крапки, без візерунка)

                SELF-VALIDATION (MANDATORY):
                Before returning result, check:

                Is JSON valid?
                Are ALL keys unchanged?
                Are ALL values Ukrainian?
                Are colors from allowed list only?
                Are flower categories from allowed list only?

                If ANY rule is violated → regenerate internally until valid.

                Return JSON ONLY in this structure:
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
                "кругла" => "Round bouquet with a balanced, symmetrical composition.",
                "подовжена" => "Elongated vertical bouquet with elegant height.",
                "горизонтальна" => "Horizontal bouquet with wide, low-profile composition.",
                "асиметрична" => "Asymmetrical bouquet with a natural, organic flow.",
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
