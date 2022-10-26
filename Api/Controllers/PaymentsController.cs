﻿using Infrastructure.Persistence;
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
            _context.PaymentMethods.Attach(PaymentMethod.Efectivo);
            _context.PaymentMethods.Attach(PaymentMethod.Transferencia);
            _context.PaymentMethods.Attach(PaymentMethod.Cheque);
            _context.PaymentMethods.Attach(PaymentMethod.Deposito);
        }

        [HttpPost]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> Create(PaymentRequest request)
        {
            var response = new Response<Payment>();
            try
            {
                User.RecoverClaims(out int personIdToken, out Guid tokenId);

                var contract = await _context.Contracts
                    .SingleOrDefaultAsync(o => o.ContractId == request.ContractId);

                var paymentNumber = await _context.Payments
                    .Where(o => o.ContractId == request.ContractId)
                    .OrderByDescending(o => o.PaymentNumber)
                    .Select(o => o.PaymentNumber)
                    .FirstOrDefaultAsync();

                var payment = new Payment
                {
                    ContractId = request.ContractId,
                    PaymentNumber = paymentNumber + 1,
                    Description = request.Description,
                    PreviousBalancePaid = contract.BalancePaid,
                    PreviousBalancePayable = contract.BalancePayable,
                    ReceiverId = personIdToken,
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

                payment.NewBalancePaid = contract.BalancePaid + payment.TotalAmount;
                payment.NewBalancePayable = contract.BalancePayable - payment.TotalAmount;

                contract.BalancePaid = payment.NewBalancePaid;
                contract.BalancePayable = payment.NewBalancePayable;

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

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
