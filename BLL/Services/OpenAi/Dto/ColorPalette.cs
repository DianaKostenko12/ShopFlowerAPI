namespace BLL.Services.OpenAi.Dto
{
    public sealed record ColorPalette
    (
        List<ColorPreference> Primary,
        List<ColorPreference> Accent
    );
}
