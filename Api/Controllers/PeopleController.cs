using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PeopleController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> People()
        {
            var response = new Response<List<Person>>();
            try
            {
                if (!User.TokenIsAccess())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var people = await _context.People
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = people;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpGet]
        [Route("{personId}")]
        public async Task<ActionResult> Person(int personId)
        {
            var response = new Response<Person>();
            try
            {
                User.RecoverClaims(out int personIdToken, out string rols, out Guid tokenId);

                if (!User.TokenIsAccess())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                if (!rols.Contains(Rol.Administrador.Name))
                {
                    personId = personIdToken;
                }

                var person = await _context.People
                    .Include(o => o.Address.City.State)
                    .SingleOrDefaultAsync(o => o.PersonId == personId);

                if (person == null)
                {
                    if (!rols.Contains(Rol.Administrador.Name))
                    {
                        response.Message = ResponseMessage.AnErrorHasOccurred;
                        return Ok(response);
                    }
                    response.Code = ResponseCode.NotFound;
                    response.Message = ResponseMessage.ResourceNotFound;
                    return Ok(response);
                }

                response.Code = ResponseCode.Ok;
                response.Data = person;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpGet]
        [Route("{personId}/[action]")]
        public async Task<ActionResult> Contracts(int personId)
        {
            var response = new Response<List<Contract>>();
            try
            {
                User.RecoverClaims(out int personIdToken, out string rols, out Guid tokenId);

                if (!User.TokenIsAccess())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                if (!rols.Contains(Rol.Administrador.Name))
                {
                    personId = personIdToken;
                }

                var contracts = await _context.Contracts
                    .Include(o => o.Property)
                    .Where(o => o.PersonId == personId)
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = contracts;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }
    }
}
