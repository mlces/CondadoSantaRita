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
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ContractsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{contractId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Contract(int contractId)
        {
            if (User.TokenIsReset())
            {
                return Unauthorized();
            }
            var response = new Response<Contract>();
            try
            {
                var contract = await _context.Contracts
                    .Include(o => o.Property)
                    .Include(o => o.PaymentPlan)
                    .FirstOrDefaultAsync(o => o.ContractId == contractId);

                if (contract == null)
                {
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                response.Code = 0;
                response.Data = contract;
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
        [Route("{contractId}/[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Payments(int contractId)
        {
            if (User.TokenIsReset())
            {
                return Unauthorized();
            }
            var response = new Response<List<Payment>>();
            try
            {
                var payments = await _context.Payments
                    .Include(o => o.Receiver)
                    .Include(o => o.Payer)
                    .Where(o => o.ContractId == contractId)
                    .ToListAsync();

                response.Code = 0;
                response.Data = payments;
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
