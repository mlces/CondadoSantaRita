using Api.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TokensController : ControllerBase, IController
    {
        private readonly ApplicationContext _context;

        private readonly TokenManager _tokenManager;

        public int PersonId { get; set; }

        public Guid TokenId { get; set; }

        public TokensController(ApplicationContext context, TokenManager tokenManager)
        {
            _context = context;
            _tokenManager = tokenManager;
            _context.TokenTypes.Attach(TokenType.Access);
            _context.TokenTypes.Attach(TokenType.Reset);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Token(TokenRequest request)
        {
            var response = new Response<Token>();
            try
            {
                var user = await _context.Users
                    .Include(o => o.Rols)
                    .SingleOrDefaultAsync(o => o.Username == request.Username);

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

                if ((bool)user.ResetPassword)
                {
                    if (user.PasswordHash == request.Password)
                    {
                        response.Code = ResponseCode.Conflict;
                        response.Message = ResponseMessage.EnterANewPassword;
                        response.Data = _tokenManager.GenerateToken(user, TokenType.Reset);

                        await _context.Database.ExecuteSqlRawAsync($"UPDATE [Token] SET [Disabled] = 1 WHERE [PersonID] = {user.PersonId}");

                        await _context.Tokens.AddAsync(response.Data);
                        await _context.SaveChangesAsync();
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
                    response.Data = _tokenManager.GenerateToken(user, TokenType.Access);

                    await _context.Database.ExecuteSqlRawAsync($"UPDATE [Token] SET [Disabled] = 1 WHERE [PersonID] = {user.PersonId}");

                    await _context.Tokens.AddAsync(response.Data);
                    await _context.SaveChangesAsync();
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpPost]
        [Route("[action]")]
        [MyAuthorize(nameof(TokenType.Reset))]
        public async Task<ActionResult> Reset(PasswordRequest request)
        {
            var response = new Response<Token>();
            try
            {
                var token = await _context.Tokens
                    .AsNoTracking()
                    .Where(o => o.Disabled == false)
                    .SingleOrDefaultAsync(o => o.TokenId == TokenId);

                if (token == null)
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.SessionHasBeenClosed;
                    return Ok(response);
                }

                var user = await _context.Users
                    .Include(o => o.Rols)
                    .FirstOrDefaultAsync(o => o.PersonId == PersonId);

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

                if (!(bool)user.ResetPassword)
                {
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                PasswordHasher<User> passwordHasher = new();
                user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
                user.ResetPassword = false;
                response.Code = ResponseCode.Ok;
                response.Data = _tokenManager.GenerateToken(user, TokenType.Access);

                await _context.Tokens.AddAsync(response.Data);
                await _context.SaveChangesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }
    }
}
