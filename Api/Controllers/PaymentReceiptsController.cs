﻿using Api.Reports;
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
    public class PaymentReceiptsController : ControllerBase, IController
    {
        private readonly PaymentReceiptsManager _paymentReceiptsManager;

        public int PersonId { get; set; }

        public Guid TokenId { get; set; }

        public ApplicationContext DbContext { get; set; }

        public PaymentReceiptsController(PaymentReceiptsManager paymentReceiptsManager, ApplicationContext dbContext)
        {
            _paymentReceiptsManager = paymentReceiptsManager;
            DbContext = dbContext;
        }

        [HttpGet]
        [Route("{paymentId}")]
        public async Task<ActionResult> PaymentReceipt(int paymentId)
        {
            var response = new Response<PaymentReceipt>();
            try
            {
                if (User.IsInRole(Rol.Administrador.Name))
                {
                    PersonId = default;
                }

                var paymentReceipt = await DbContext.PaymentReceipts
                    .Include(o => o.Payment.Contract)
                    .Where(o => o.Payment.Contract.PersonId == (PersonId != default ? PersonId : o.Payment.Contract.PersonId))
                    .SingleOrDefaultAsync(o => o.PaymentId == paymentId);

                if (paymentReceipt == null)
                {
                    var payment = await DbContext.Payments
                        .Include(o => o.Payer)
                        .Include(o => o.Receiver)
                        .Include(o => o.Contract)
                        .ThenInclude(o => o.Property)
                        .Where(o => o.Contract.PersonId == (PersonId != default ? PersonId : o.Contract.PersonId))
                        .SingleOrDefaultAsync(o => o.PaymentId == paymentId);

                    if (payment == null)
                    {
                        response.Message = ResponseMessage.AnErrorHasOccurred;
                        return Ok(response);
                    }

                    var paymentDetails = await DbContext.PaymentDetails
                        .Include(o => o.Bank)
                        .Include(o => o.Payment.Contract)
                        .Where(o => o.PaymentId == paymentId && o.Payment.Contract.PersonId == (PersonId != default ? PersonId : o.Payment.Contract.PersonId))
                        .ToListAsync();

                    var report = _paymentReceiptsManager.Generate(payment, paymentDetails);

                    paymentReceipt = new()
                    {
                        PaymentId = paymentId,
                        Data = report
                    };

                    DbContext.PaymentReceipts.Add(paymentReceipt);
                    DbContext.SaveChanges();
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
