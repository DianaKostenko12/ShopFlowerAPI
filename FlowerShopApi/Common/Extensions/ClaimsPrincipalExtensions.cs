using System.Security.Claims;

namespace FlowerShopApi.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal principal)
        {
            var userIdString = GetInfoByDataName(principal, "userId");

            if (!int.TryParse(userIdString, out int userId))
            {
                return null;
            }

            return userId;
        }

        private static string GetInfoByDataName(ClaimsPrincipal principal, string name)
        {
            return principal.FindFirstValue(name);
        }
    }
}
