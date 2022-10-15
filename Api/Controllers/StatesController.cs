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
    public class StatesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public StatesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> States()
        {
            if (User.TokenIsReset())
            {
                return Unauthorized();
            }
            var response = new Response<IEnumerable<State>>();
            try
            {
                var states = await _context.States
                    .Include(o => o.Cities)
                    .ToListAsync();

                response.Code = 0;
                response.Data = states;
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
