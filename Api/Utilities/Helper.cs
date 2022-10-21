using Humanizer;
using System.Security.Claims;

namespace Api.Utilities
{
    public static class Helper
    {
        public static string ToCurrency(this decimal amount)
        {
            return string.Format("{0:Q0,0.00}", amount);
        }

        public static string ToWords(this decimal amount)
        {
            var units = (int)Math.Truncate(amount);
            var result = $"{units.ToWords()} quetzales";
            var decimals = (int)((amount - units) * 100);
            if (decimals == 0)
            {
                result += " exactos";
            }
            else
            {
                result += $" con {decimals.ToWords()} centavos";
            }
            result = result.Replace("uno", "un");
            return result.Humanize(LetterCasing.Sentence);
        }

        public static bool TokenIsReset(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Version)?.Value == TokenType.Reset.Name;
        }

        public static bool TokenIsAccess(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Version)?.Value == TokenType.Access.Name;
        }

        public static Response<T> GenerateError<T>(this Response<T> response, Exception ex)
        {
            var errorId = Configuration.LogError(ex.ToString());
            response.Code = ResponseCode.BadRequest;
            response.Message = ResponseMessage.AnErrorHasOccurredAndId(errorId);
            response.Data = default;
            return response;
        }

        public static void RecoverClaims(this ClaimsPrincipal user, out int personId, out string rols, out Guid tokenId)
        {
            Claim? claim = user.FindFirst(ClaimTypes.Actor);
            personId = claim != null ? int.Parse(claim.Value) : default;

            var claims = user.Claims.Where(o => o.Type == ClaimTypes.Role).ToList();
            rols = string.Empty;
            foreach (var item in claims)
            {
                rols += item.Value;
            }

            claim = user.FindFirst(ClaimTypes.Sid);
            tokenId = claim != null ? new(claim.Value) : default;
        }
    }
}