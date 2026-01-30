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
