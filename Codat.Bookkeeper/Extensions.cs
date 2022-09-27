using System.Security.Claims;

namespace Codat.Bookkeeper
{
    public static class Extensions
    {
        public static int GetTenantId(this ClaimsPrincipal principal)
        {
            return int.Parse(principal.FindFirstValue("TenantId"));
        }
    }
}
