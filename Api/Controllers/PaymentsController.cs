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
            if (User.TokenIsReset())
            {
                return Unauthorized();
            }
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
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                response.Code = 0;
                response.Data = paymentDetails;
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
