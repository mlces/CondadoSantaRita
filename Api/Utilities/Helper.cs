using Api.Tokens;
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
            try
            {
                var userTokenType = user.FindFirst(ClaimTypes.Version).Value.ToString();
                if (userTokenType == TokenType.ResetPassword.ToString())
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}