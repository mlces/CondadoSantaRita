using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Tokens
{
    public class TokenManager
    {
        public enum TokenType
        {
            Login,
            ResetPassword
        }

        public TokenResponse<User> GenerateToken(User user, TokenType tokenType)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Actor, user.PersonId.ToString())
            };
            foreach (var rol in user.Rols)
            {
                claims.Add(new(ClaimTypes.Role, rol.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.TokenSigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now;

            switch (tokenType)
            {
                case TokenType.Login:
                    expires = expires.AddMinutes(Configuration.TokenValidityMinutesLogin);
                    claims.Add(new(ClaimTypes.Version, nameof(TokenType.Login)));
                    break;
                case TokenType.ResetPassword:
                    expires = expires.AddMinutes(Configuration.TokenValidityMinutesResetPassword);
                    claims.Add(new(ClaimTypes.Version, nameof(TokenType.ResetPassword)));
                    break;
            }

            JwtSecurityToken token = new(
                issuer: Configuration.TokenIssuer,
                audience: Configuration.TokenAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
                );

            return new TokenResponse<User>()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = expires,
                Data = user
            };
        }
    }
}
