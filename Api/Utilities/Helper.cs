using Api.Tokens;
using Humanizer;
using Microsoft.AspNetCore.Identity;
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

        public static Response<T> GenerateError<T>(this Response<T> response, Exception ex)
        {
            var errorId = Configuration.LogError(ex.ToString());
            response.Code = ResponseCode.BadRequest;
            response.Message = ResponseMessage.AnErrorHasOccurredAndId(errorId);
            response.Data = default;
            return response;
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