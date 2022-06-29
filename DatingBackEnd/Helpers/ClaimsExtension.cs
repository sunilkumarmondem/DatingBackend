using System.Security.Claims;

namespace DatingBackEnd.Helpers
{
    public static class ClaimsExtension
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user?.Identity?.Name;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
