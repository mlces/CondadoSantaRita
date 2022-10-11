using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public UsersController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Token(TokenRequest request)
        {
            var response = new Response<TokenResponse<User>>();

            try
            {
                var user = await _context.Users
                    .Include(o => o.Rols)
                    .FirstOrDefaultAsync(o => o.Username == request.Username && o.Disabled == false);

                if (user == null)
                {
                    response.Message = "Las credenciales son incorrectas.";
                    return Ok(response);
                }

                PasswordHasher<User> passwordHasher = new();

                if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                {
                    response.Message = "Las credenciales son incorrectas.";
                    return Ok(response);
                }

                return GenerateToken(user);
            }
            catch (Exception ex)
            {
                var errorId = Configuration.LogError(ex.ToString());
                response.Message = $"Ha ocurrido un error, intente nuevamente o reporte el error: {errorId}.";
                return Ok(response);
            }
        }

        private ActionResult GenerateToken(User user)
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
            var expires = DateTime.Now.AddHours(Configuration.TokenValidityHours);

            JwtSecurityToken token = new(
                issuer: Configuration.TokenIssuer,
                audience: Configuration.TokenAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
                );

            return Ok(new Response<TokenResponse<User>>()
            {
                Code = 0,
                Message = null,
                Data = new()
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiresIn = expires,
                    Data = user
                }
            });
        }
    }
}
