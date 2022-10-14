using Api.Tokens;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Route("[action]")]
        public async Task<ActionResult> RequestToken(TokenRequest request)
        {
            var response = new Response<TokenResponse<User>>();
            TokenManager tokenManager = new();
            try
            {
                var user = await _context.Users
                    .Include(o => o.Rols)
                    .FirstOrDefaultAsync(o => o.Username == request.Username);

                if (user == null)
                {
                    response.Message = "Las credenciales son incorrectas.";
                    return Ok(response);
                }

                if (user.Disabled)
                {
                    response.Message = "Usuario deshabilitado, contacta a un administrador.";
                    return Ok(response);
                }

                if (user.ResetPassword)
                {
                    if (user.PasswordHash == request.Password)
                    {
                        response.Code = 1;
                        response.Message = "Ingresa una nueva contraseña para poder acceder.";
                        response.Data = tokenManager.GenerateToken(user, TokenManager.TokenType.ResetPassword);
                        return Ok(response);
                    }
                    else
                    {
                        response.Message = "Las credenciales son incorrectas.";
                        return Ok(response);
                    }
                }
                else
                {
                    PasswordHasher<User> passwordHasher = new();

                    if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                    {
                        response.Message = "Las credenciales son incorrectas.";
                        return Ok(response);
                    }

                    response.Code = 0;
                    response.Data = tokenManager.GenerateToken(user, TokenManager.TokenType.Login);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                var errorId = Configuration.LogError(ex.ToString());
                response.Code = -1;
                response.Message = $"Ha ocurrido un error, intente nuevamente o reporte el error: {errorId}.";
                return Ok(response);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ResetPassword(PasswordRequest request)
        {
            var response = new Response<TokenResponse<User>>();
            TokenManager tokenManager = new();
            try
            {
                if (User.TokenIsLogin())
                {
                    response.Message = "Ha ocurrido un error, intenta nuevamente.";
                    return Ok(response);
                }

                var user = await _context.Users
                    .Include(o => o.Rols)
                    .FirstOrDefaultAsync(o => o.Username == request.Username);

                if (user == null)
                {
                    response.Message = "Las credenciales son incorrectas.";
                    return Ok(response);
                }

                if (user.Disabled)
                {
                    response.Message = "Usuario deshabilitado, contacta a un administrador.";
                    return Ok(response);
                }

                PasswordHasher<User> passwordHasher = new();
                user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
                user.ResetPassword = false;
                _context.Update(user);
                await _context.SaveChangesAsync();

                response.Code = 0;
                response.Data = tokenManager.GenerateToken(user, TokenManager.TokenType.Login);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorId = Configuration.LogError(ex.ToString());
                response.Code = -1;
                response.Message = $"Ha ocurrido un error, intente nuevamente o reporte el error: {errorId}.";
                return Ok(response);
            }
        }
    }
}