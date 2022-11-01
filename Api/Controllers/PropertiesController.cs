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
    public class PropertiesController : ControllerBase, IController
    {
        public int PersonId { get; set; }

        public int TokenId { get; set; }

        public ApplicationContext DbContext { get; set; }

        public PropertiesController(ApplicationContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> WithoutAgreement()
        {
            var response = new Response<List<Property>>();
            try
            {
                var properties = await DbContext.Properties
                    .AsNoTracking()
                    .Where(o => o.Agreement == null)
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = properties;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpGet]
        [Route("[action]/{propertyId}")]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> WithoutAgreement(int propertyId)
        {
            var response = new Response<Property>();
            try
            {
                var property = await DbContext.Properties
                    .AsNoTracking()
                    .SingleOrDefaultAsync(o => o.PropertyId == propertyId);

                response.Code = ResponseCode.Ok;
                response.Data = property;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }
    }
}
