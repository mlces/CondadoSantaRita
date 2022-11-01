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
    public class PaymentsController : ControllerBase, IController
    {
        public int PersonId { get; set; }

        public int TokenId { get; set; }

        public ApplicationContext DbContext { get; set; }

        public PaymentsController(ApplicationContext dbContext)
        {
            DbContext = dbContext;
            DbContext.PaymentMethods.Attach(PaymentMethod.Efectivo);
            DbContext.PaymentMethods.Attach(PaymentMethod.Transferencia);
            DbContext.PaymentMethods.Attach(PaymentMethod.Cheque);
            DbContext.PaymentMethods.Attach(PaymentMethod.Deposito);
        }

        [HttpPost]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> Create(PaymentRequest request)
        {
            var response = new Response<Payment>();
            try
            {
                var Agreement = await DbContext.Agreements
                    .SingleOrDefaultAsync(o => o.PropertyId == request.PropertyId);

                var paymentNumber = await DbContext.Payments
                    .Where(o => o.PropertyId == request.PropertyId)
                    .OrderByDescending(o => o.PaymentNumber)
                    .Select(o => o.PaymentNumber)
                    .FirstOrDefaultAsync();

                var payment = new Payment
                {
                    PropertyId = request.PropertyId,
                    PaymentNumber = paymentNumber + 1,
                    Description = request.Description,
                    PreviousBalancePaid = Agreement.BalancePaid,
                    PreviousBalancePayable = Agreement.BalancePayable,
                    ReceiverId = PersonId,
                    PayerId = request.PayerId
                };

                foreach (var item in request.PaymentDetails)
                {
                    if (item.Amount > 0)
                    {
                        var paymentDetails = new PaymentDetail()
                        {
                            PaymentMethodId = item.PaymentMethodId,
                            Amount = item.Amount

                        };

                        if (item.PaymentMethodId != PaymentMethod.Efectivo.PaymentMethodId)
                        {
                            paymentDetails.BankId = item.BankId;
                            paymentDetails.ReferenceNumber = item.ReferenceNumber;
                        }

                        payment.PaymentDetails.Add(paymentDetails);

                        payment.TotalAmount += item.Amount;
                    }
                }

                if (payment.TotalAmount <= 0)
                {
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                payment.NewBalancePaid = Agreement.BalancePaid + payment.TotalAmount;
                payment.NewBalancePayable = Agreement.BalancePayable - payment.TotalAmount;

                Agreement.BalancePaid = payment.NewBalancePaid;
                Agreement.BalancePayable = payment.NewBalancePayable;

                payment.PaymentId = await DbContext.GetPaymentIdentifier();

                await DbContext.Payments.AddAsync(payment);
                await DbContext.SaveChangesAsync();

                response.Code = ResponseCode.Ok;
                response.Data = payment;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }
    }
}
