namespace BLL.Services.OpenAi.Dto
{
    public sealed record WrappingRecommendation
    (
        List<string> Materials,
        List<string> Colors,
        List<string> Patterns
    );
}
