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
        public int PersonId { get; set; }

        public Guid TokenId { get; set; }

        public ApplicationContext DbContext { get; set; }

        public PeopleController(ApplicationContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> People()
        {
            var response = new Response<List<Person>>();
            try
            {
                var people = await DbContext.People
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

                var person = await DbContext.People
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

                var contracts = await DbContext.Contracts
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

        [HttpPost]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> Create(PersonRequest personRequest)
        {
            var response = new Response<Person>();
            try
            {
                Person person = new()
                {
                    FirstName = personRequest.FirstName,
                    LastName = personRequest.LastName,
                    EmailAddress = personRequest.EmailAddress,
                    PhoneNumber = personRequest.PhoneNumber,
                    NationalId = personRequest.NationalId,
                    Birthday = personRequest.Birthday,
                    Address = new()
                    {
                        Name = personRequest.AddressName,
                        CityId = personRequest.CityId
                    }
                };

                await DbContext.People.AddAsync(person);
                await DbContext.SaveChangesAsync();

                response.Code = ResponseCode.Ok;
                response.Data = person;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }
    }
}
