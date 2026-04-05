namespace BLL.Services.OpenAi.Dto
{
    public sealed record WrappingRecommendation
    (
        List<ColorPreference> Colors,
        string Type,
        string Pattern
    );
}
