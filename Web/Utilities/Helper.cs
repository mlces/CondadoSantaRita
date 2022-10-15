using Api.Tokens;
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