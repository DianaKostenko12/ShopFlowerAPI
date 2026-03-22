namespace BLL.Services.OpenAi.Dto
{
    public sealed record GptStyleRecommendation
    (
        string BouquetName,
        ColorPalette Palette,           
        FlowerRoles Roles,              
        WrappingRecommendation WrappingPaper
    );
}
