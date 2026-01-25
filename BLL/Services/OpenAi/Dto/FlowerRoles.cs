namespace BLL.Services.OpenAi.Dto
{
    public sealed record FlowerRoles
    (
        RoleSpec Focal,
        RoleSpec Semi,
        RoleSpec Filler,
        RoleSpec Greenery
    );
}
