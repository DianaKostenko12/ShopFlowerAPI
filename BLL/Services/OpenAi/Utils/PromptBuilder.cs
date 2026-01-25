using BLL.Services.BouquetGeneration.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.OpenAi.Utils
{
    internal static class PromptBuilder
    {
        internal static string BuildStylePrompt(GenerateBouquetDescriptor bouquet)
        {
           return $$"""
                You are a florist assistant.

                Style: {{bouquet.Style}}
                Color palette: {{bouquet.Color}}

                Task:
                Suggest suitable flower CATEGORIES for this bouquet style.

                Rules:
                - Use only generic categories (e.g. Rose, Peony, Tulip, Eustoma, Greenery).
                - Max 2 focal categories.
                - Max 2 filler categories.
                - Max 1 greenery category.
                - Do NOT mention specific products or prices.

                Return STRICT JSON in this format:
                {
                  "focalCategories": [],
                  "fillerCategories": [],
                  "greeneryCategories": []
                }
            """;
        }
    }
}
