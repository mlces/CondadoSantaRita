using Api.Tokens;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;

        private readonly TokenManager _tokenManager;

        public UsersController(ApplicationContext context, TokenManager tokenManager)
        {
            _context = context;
            _tokenManager = tokenManager;
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> RequestToken(TokenRequest request)
        {
            var response = new Response<TokenResponse<User>>();

            try
            {
                var user = await _context.Users
                    .Include(o => o.Rols)
                    .FirstOrDefaultAsync(o => o.Username == request.Username);

                if (user == null)
                {
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                if (user.Disabled)
                {
                    response.Message = ResponseMessage.DisabledUser;
                    return Ok(response);
                }

                if (user.ResetPassword)
                {
                    if (user.PasswordHash == request.Password)
                    {
                        response.Code = ResponseCode.Conflict;
                        response.Message = ResponseMessage.EnterANewPassword;
                        response.Data = _tokenManager.GenerateToken(user, TokenType.ResetPassword);
                        return Ok(response);
                    }
                    else
                    {
                        response.Message = ResponseMessage.WrongCredentials;
                        return Ok(response);
                    }
                }
                else
                {
                    PasswordHasher<User> passwordHasher = new();

                    if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                    {
                        response.Message = ResponseMessage.WrongCredentials;
                        return Ok(response);
                    }

                    response.Code = ResponseCode.Ok;
                    response.Data = _tokenManager.GenerateToken(user, TokenType.Login);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                var errorId = Configuration.LogError(ex.ToString());
                response.Code = ResponseCode.BadRequest;
                response.Message = ResponseMessage.AnErrorHasOccurredAndId(errorId);
                return Ok(response);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> ResetPassword(PasswordRequest request)
        {
            var response = new Response<TokenResponse<User>>();
            try
            {
                if (!User.TokenIsReset())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var personId = int.Parse(User.FindFirst(ClaimTypes.Actor).Value.ToString());

                var user = await _context.Users
                    .Include(o => o.Rols)
                    .FirstOrDefaultAsync(o => o.PersonId == personId);

                if (user == null)
                {
                    response.Message = ResponseMessage.WrongCredentials;
                    return Ok(response);
                }

                if (user.Disabled)
                {
                    response.Message = ResponseMessage.DisabledUser;
                    return Ok(response);
                }

                PasswordHasher<User> passwordHasher = new();
                user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
                user.ResetPassword = false;
                _context.Update(user);
                await _context.SaveChangesAsync();

                response.Code = ResponseCode.Ok;
                response.Data = _tokenManager.GenerateToken(user, TokenType.Login);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorId = Configuration.LogError(ex.ToString());
                response.Code = ResponseCode.BadRequest;
                response.Message = ResponseMessage.AnErrorHasOccurredAndId(errorId);
                return Ok(response);
            }
        }
    }
}