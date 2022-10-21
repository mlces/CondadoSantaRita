using Api.Reports;
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
                User.RecoverClaims(out int personIdToken, out string rols, out Guid tokenId);

                if (!User.TokenIsAccess())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                if (User.IsInRole(Rol.Administrador.Name))
                {
                    personIdToken = default;
                }

                var paymentReceipt = await _context.PaymentReceipts
                    .Include(o => o.Payment.Contract)
                    .Where(o => o.Payment.Contract.PersonId == (personIdToken != default ? personIdToken : o.Payment.Contract.PersonId))
                    .SingleOrDefaultAsync(o => o.PaymentId == paymentId);

                if (paymentReceipt == null)
                {
                    var payment = await _context.Payments
                        .Include(o => o.Payer)
                        .Include(o => o.Receiver)
                        .Include(o => o.Contract)
                        .Where(o => o.Contract.PersonId == (personIdToken != default ? personIdToken : o.Contract.PersonId))
                        .SingleOrDefaultAsync(o => o.PaymentId == paymentId);

                    if (payment == null)
                    {
                        response.Message = ResponseMessage.AnErrorHasOccurred;
                        return Ok(response);
                    }

                    var paymentDetails = await _context.PaymentDetails
                        .Include(o => o.Bank)
                        .Include(o => o.Payment.Contract)
                        .Where(o => o.PaymentId == paymentId && o.Payment.Contract.PersonId == (personIdToken != default ? personIdToken : o.Payment.Contract.PersonId))
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
                return Ok(response.GenerateError(ex));
            }
        }
    }
}
