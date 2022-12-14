using System.Security.Claims;

namespace Web.Utilities
{
    public static class Helper
    {
        public static string ToCurrency(this decimal amount)
        {
            return string.Format("{0:Q0,0.00}", amount);
        }

        public static bool TokenIsReset(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Version)?.Value == TokenType.Reset.Name;
        }

        public static bool TokenIsAccess(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Version)?.Value == TokenType.Access.Name;
        }

        public static void RecoverClaims(this ClaimsPrincipal user, out int personId, out Guid tokenId)
        {
            Claim? claim = user.FindFirst(ClaimTypes.Actor);
            personId = claim != null ? int.Parse(claim.Value) : default;

            claim = user.FindFirst(ClaimTypes.Sid);
            tokenId = claim != null ? new(claim.Value) : default;
        }
    }
}