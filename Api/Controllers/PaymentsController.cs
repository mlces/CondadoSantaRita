using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PaymentsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{paymentId}/PaymentsDetails")]
        public async Task<ActionResult> PaymentsDetails(int paymentId)
        {
            var response = new Response<List<PaymentDetail>>();
            try
            {
                var paymentDetails = await _context.PaymentDetails
                    .Include(o => o.Bank)
                    .Include(o => o.PaymentMethod)
                    .Where(o => o.PaymentId == paymentId)
                    .ToListAsync();

                if (paymentDetails == null)
                {
                    response.Message = "Ha ocurrido un error, intente nuevamente.";
                    return Ok(response);
                }

                response.Code = 0;
                response.Data = paymentDetails;
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
