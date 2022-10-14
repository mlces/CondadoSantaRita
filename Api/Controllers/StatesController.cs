using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                response.Code = -1;
                response.Message = $"Ha ocurrido un error, intente nuevamente o reporte el error: {errorId}.";
                return Ok(response);
            }
        }
    }
}
