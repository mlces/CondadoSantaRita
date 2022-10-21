using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Tokens
{
    public class TokenManager
    {
        public Token GenerateToken(User user, TokenType tokenType)
        {
            var guid = Guid.NewGuid();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Actor, user.PersonId.ToString()),
                new(ClaimTypes.Sid, guid.ToString()),
            };
            foreach (var rol in user.Rols)
            {
                claims.Add(new(ClaimTypes.Role, rol.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.TokenSigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now;

            if (tokenType == TokenType.Access)
            {
                expires = expires.AddMinutes(Configuration.TokenValidityMinutesAccess);
                claims.Add(new(ClaimTypes.Version, TokenType.Access.Name));
            }
            else if (tokenType == TokenType.Reset)
            {
                expires = expires.AddMinutes(Configuration.TokenValidityMinutesReset);
                claims.Add(new(ClaimTypes.Version, TokenType.Reset.Name));
            }

            JwtSecurityToken token = new(
                issuer: Configuration.TokenIssuer,
                audience: Configuration.TokenAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
                );

            return new Token()
            {
                TokenId = guid,
                TokenTypeId = tokenType.TokenTypeId,
                Data = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = expires,
                PersonId = user.PersonId
            };
        }
    }
}
