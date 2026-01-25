namespace BLL.Services.OpenAi.Dto
{
    public sealed record ColorPalette
    (
        List<string> Primary,
        List<string> Accent 
    );
}
