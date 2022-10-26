using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public UsersController(ApplicationContext context)
        {
            _context = context;
            _context.Rols.Attach(Rol.Cliente);
            _context.Rols.Attach(Rol.Administrador);
        }

        [HttpPost]
        [Route("{personId}")]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> Create(int personId)
        {
            var response = new Response<User>();
            try
            {
                if (!User.TokenIsAccess())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var person = await _context.People
                    .Where(o => o.PersonId == personId)
                    .SingleOrDefaultAsync();

                if (person == null)
                {
                    response.Code = ResponseCode.NotFound;
                    response.Message = ResponseMessage.ResourceNotFound;
                    return Ok(response);
                }

                var password = GeneratePassword();

                User user = new()
                {
                    PersonId = person.PersonId,
                    Username = person.EmailAddress,
                    PasswordHash = password,
                    Password = password,
                };

                user.Rols.Add(Rol.Cliente);

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                response.Code = ResponseCode.Ok;
                response.Data = user;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpPost]
        [Route("{personId}/[action]")]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> Reset(int personId)
        {
            var response = new Response<User>();
            try
            {
                if (!User.TokenIsAccess())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var user = await _context.Users
                    .Where(o => o.PersonId == personId)
                    .SingleOrDefaultAsync();

                if (user == null)
                {
                    response.Code = ResponseCode.NotFound;
                    response.Message = ResponseMessage.ResourceNotFound;
                    return Ok(response);
                }

                var password = GeneratePassword();

                user.PasswordHash = password;
                user.Password = password;
                user.ResetPassword = true;

                await _context.SaveChangesAsync();

                response.Code = ResponseCode.Ok;
                response.Data = user;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        private string GeneratePassword()
        {
            StringBuilder builder = new();
            Random random = new();
            char ch;
            for (int i = 0; i < 8; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
