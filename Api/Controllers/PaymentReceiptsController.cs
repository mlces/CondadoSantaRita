using Api.Reports;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentReceiptsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        private readonly PaymentReceiptsManager _paymentReceiptsManager;

        public PaymentReceiptsController(ApplicationContext context, PaymentReceiptsManager paymentReceiptsManager)
        {
            _context = context;
            _paymentReceiptsManager = paymentReceiptsManager;
        }

        [HttpGet]
        [Route("{paymentId}")]
        public async Task<ActionResult> PaymentReceipt(int paymentId)
        {
            var response = new Response<PaymentReceipt>();
            try
            {
                if (User.TokenIsReset())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var personId = int.Parse(User.FindFirst(ClaimTypes.Actor).Value.ToString());

                var paymentReceipt = await _context.PaymentReceipts
                    .FirstOrDefaultAsync(o => o.PaymentId == paymentId);

                if (paymentReceipt == null)
                {
                    var payment = await _context.Payments
                        .Include(o => o.Payer)
                        .Include(o => o.Receiver)
                        .FirstOrDefaultAsync(o => o.PaymentId == paymentId);

                    if (payment == null)
                    {
                        response.Message = ResponseMessage.AnErrorHasOccurred;
                        return Ok(response);
                    }

                    var paymentDetails = await _context.PaymentDetails
                        .Where(o => o.PaymentId == paymentId)
                        .Include(o => o.Bank)
                        .ToListAsync();

                    var report = _paymentReceiptsManager.Generate(payment, paymentDetails);

                    paymentReceipt = new()
                    {
                        PaymentId = paymentId,
                        Data = report
                    };

                    _context.PaymentReceipts.Add(paymentReceipt);
                    _context.SaveChanges();
                }

                response.Code = ResponseCode.Ok;
                response.Data = paymentReceipt;
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
