using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CatalogsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public CatalogsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> Banks()
        {
            var response = new Response<List<Bank>>();
            try
            {
                var banks = await _context.Banks
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = banks;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }
    }
}
