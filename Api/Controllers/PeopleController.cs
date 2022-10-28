using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [MyAuthorize]
    public class PeopleController : ControllerBase, IController
    {
        private readonly ApplicationContext _context;

        public int PersonId { get; set; }

        public Guid TokenId { get; set; }

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
                if (!User.IsInRole(Rol.Administrador.Name))
                {
                    personId = PersonId;
                }

                var person = await _context.People
                    .Include(o => o.Address.City.State)
                    .Include(o => o.User)
                    .SingleOrDefaultAsync(o => o.PersonId == personId);

                if (person == null)
                {
                    if (!User.IsInRole(Rol.Administrador.Name))
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
                if (!User.IsInRole(Rol.Administrador.Name))
                {
                    personId = PersonId;
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
