namespace BLL.Services.OpenAi.Dto
{
    public sealed record WrappingRecommendation
    (
        List<string> Colors,
        List<string> Patterns
    );
}
