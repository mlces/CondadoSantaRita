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
        [Route("{personId}")]
        public async Task<ActionResult> People(int personId)
        {
            if (User.TokenIsReset())
            {
                return Unauthorized();
            }
            var response = new Response<Person>();
            try
            {
                var person = await _context.People
                    .Include(o => o.Address.City.State)
                    .FirstOrDefaultAsync(o => o.PersonId == personId);

                if (person == null)
                {
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                response.Code = 0;
                response.Data = person;
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

        [HttpGet]
        [Route("{personId}/[action]")]
        public async Task<ActionResult> Contracts(int personId)
        {
            if (User.TokenIsReset())
            {
                return Unauthorized();
            }
            var response = new Response<List<Contract>>();
            try
            {
                var contracts = await _context.Contracts
                    .Include(o => o.Property)
                    .Where(o => o.PersonId == personId)
                    .ToListAsync();

                response.Code = 0;
                response.Data = contracts;
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
