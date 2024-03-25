using BlazorStack.Portal.Constants;
using System.Security.Claims;

namespace BlazorStack.Portal.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetPhotoUrl(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(x => x.Type.Equals("photo-url", StringComparison.OrdinalIgnoreCase))?.Value ?? Global.DefaultPhotoUrl;
        }
    }
}
