using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.OpenAi.Dto
{
    public sealed record GptStyleRecommendation
    (
        string Style,                   
        string Shape,                   

        ColorPalette Palette,           

        FlowerRoles Roles,              

        WrappingRecommendation WrappingPaper
    );
}
