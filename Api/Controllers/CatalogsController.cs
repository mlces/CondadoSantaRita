using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = nameof(Rol.Administrador))]
    [MyAuthorize]
    public class CatalogsController : ControllerBase, IController
    {
        public int PersonId { get; set; }

        public Guid TokenId { get; set; }

        public ApplicationContext DbContext { get; set; }

        public CatalogsController(ApplicationContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult> Banks()
        {
            var response = new Response<List<Bank>>();
            try
            {
                var banks = await DbContext.Banks
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

        [HttpGet]
        public async Task<ActionResult> States()
        {
            var response = new Response<List<State>>();
            try
            {
                var banks = await DbContext.States
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

        [HttpGet]
        public async Task<ActionResult> Citiies()
        {
            var response = new Response<List<City>>();
            try
            {
                var banks = await DbContext.Cities
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
