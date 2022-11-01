using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Tokens
{
    public class TokenManager
    {
        public ApplicationContext DbContext { get; set; }

        public TokenManager(ApplicationContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<Token> GenerateToken(User user, TokenType tokenType)
        {
            var tokenId = await DbContext.GetTokenIdentifier();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Actor, user.PersonId.ToString()),
                new(ClaimTypes.Sid, tokenId.ToString()),
                new(ClaimTypes.Version, tokenType.Name)
            };

            foreach (var rol in user.Rols)
            {
                claims.Add(new(ClaimTypes.Role, rol.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.TokenSigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now;

            if (tokenType.Name == nameof(TokenType.Access))
            {
                expires = expires.AddMinutes(Configuration.TokenValidityMinutesAccess);
            }
            else if (tokenType.Name == TokenType.Reset.Name)
            {
                expires = expires.AddMinutes(Configuration.TokenValidityMinutesReset);
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
                TokenId = tokenId,
                TokenType = tokenType,
                Data = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = expires,
                PersonId = user.PersonId
            };
        }
    }
}
