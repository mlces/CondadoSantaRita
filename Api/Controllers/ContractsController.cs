using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ContractsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{contractId}")]
        public async Task<ActionResult> Contract(int contractId)
        {
            var response = new Response<Contract>();
            try
            {
                var contract = await _context.Contracts
                    .Include(o => o.Property)
                    .Include(o => o.PaymentPlan)
                    .FirstOrDefaultAsync(o => o.ContractId == contractId);

                if (contract == null)
                {
                    response.Message = "Ha ocurrido un error, intente nuevamente.";
                    return Ok(response);
                }

                response.Code = 0;
                response.Data = contract;
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorId = Configuration.LogError(ex.ToString());
                response.Message = $"Ha ocurrido un error, intente nuevamente o reporte el error: {errorId}.";
                return Ok(response);
            }
        }

        [HttpGet]
        [Route("{contractId}/[action]")]
        public async Task<ActionResult> Payments(int contractId)
        {
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
                response.Message = $"Ha ocurrido un error, intente nuevamente o reporte el error: {errorId}.";
                return Ok(response);
            }
        }
    }
}
