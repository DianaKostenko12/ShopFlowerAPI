namespace BLL.Services.OpenAi.Dto
{
    public sealed record RoleSpec
    (
        List<string> Categories,
        int Min,
        int Max
    );
}
