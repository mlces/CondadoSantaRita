using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CatalogsController : ControllerBase
    {
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
                    .AsNoTracking()
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
                var states = await DbContext.States
                    .AsNoTracking()
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = states;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpGet]
        public async Task<ActionResult> PaymentPlans()
        {
            var response = new Response<List<PaymentPlan>>();
            try
            {
                var paymentPlans = await DbContext.PaymentPlans
                    .AsNoTracking()
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = paymentPlans;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpGet]
        public async Task<ActionResult> Cities()
        {
            var response = new Response<List<City>>();
            try
            {
                var cities = await DbContext.Cities
                    .AsNoTracking()
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = cities;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }
    }
}
