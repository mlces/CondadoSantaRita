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

        public static void RecoverClaims(this ClaimsPrincipal user, out int? personId, out string? rols, out Guid? tokenId, out TokenType? tokenType)
        {
            Claim? claim = user.FindFirst(ClaimTypes.Actor);
            personId = claim != null ? int.Parse(claim.Value) : null;

            claim = user.FindFirst(ClaimTypes.Role);
            rols = claim?.Value;

            claim = user.FindFirst(ClaimTypes.Sid);
            tokenId = claim != null ? new(claim.Value) : null;

            claim = user.FindFirst(ClaimTypes.Version);
            if (claim?.Value == TokenType.Access.Name)
            {
                tokenType = TokenType.Access;
            }
            else if (claim?.Value == TokenType.Reset.Name)
            {
                tokenType = TokenType.Reset;
            }
            else if (claim?.Value == TokenType.Refresh.Name)
            {
                tokenType = TokenType.Refresh;
            }
            else
            {
                tokenType = null;
            }
        }
    }
}