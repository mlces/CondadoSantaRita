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
    public class AgreementsController : ControllerBase, IController
    {
        public int PersonId { get; set; }

        public int TokenId { get; set; }

        public ApplicationContext DbContext { get; set; }

        public AgreementsController(ApplicationContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> Agreements()
        {
            var response = new Response<List<Agreement>>();
            try
            {
                var agreements = await DbContext.Agreements
                    .Include(o => o.Person)
                    .Include(o => o.PaymentPlan)
                    .Include(o => o.Person)
                    .Include(o => o.Property)
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = agreements;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpGet]
        [Route("{propertyId}")]
        public async Task<ActionResult> Agreement(int propertyId)
        {
            var response = new Response<Agreement>();
            try
            {
                if (User.IsInRole(Rol.Administrador.Name))
                {
                    PersonId = default;
                }

                var agreement = await DbContext.Agreements
                    .Include(o => o.Person)
                    .Include(o => o.PaymentPlan)
                    .Include(o => o.Person)
                    .Include(o => o.Property)
                    .Where(o => o.PersonId == (PersonId != default ? PersonId : o.PersonId))
                    .SingleOrDefaultAsync(o => o.PropertyId == propertyId);

                if (agreement == null)
                {
                    if (!User.IsInRole(Rol.Administrador.Name))
                    {
                        response.Message = ResponseMessage.AnErrorHasOccurred;
                        return Ok(response);
                    }
                    response.Code = ResponseCode.NotFound;
                    response.Message = ResponseMessage.ResourceNotFound;
                    return Ok(response);
                }

                response.Code = ResponseCode.Ok;
                response.Data = agreement;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpGet]
        [Route("{propertyId}/[action]")]
        public async Task<ActionResult> Payments(int propertyId)
        {
            var response = new Response<List<Payment>>();
            try
            {
                if (User.IsInRole(Rol.Administrador.Name))
                {
                    PersonId = default;
                }

                var payments = await DbContext.Payments
                    .Include(o => o.Receiver)
                    .Include(o => o.Payer)
                    .Where(o => o.PropertyId == propertyId)
                    .Where(o => o.Agreement.PersonId == (PersonId != default ? PersonId : o.Agreement.PersonId))
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = payments;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpPost]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> Create(AgreementRequest AgreementRequest)
        {
            var response = new Response<Agreement>();
            try
            {
                var agreementExist = await DbContext.Agreements
                    .Where(o => o.PropertyId == AgreementRequest.PropertyId)
                    .AnyAsync();

                if (agreementExist)
                {
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var property = await DbContext.Properties
                    .SingleOrDefaultAsync(o => o.PropertyId == AgreementRequest.PropertyId);

                var paymentPlan = await DbContext.PaymentPlans
                    .SingleOrDefaultAsync(o => o.PaymentPlanId == AgreementRequest.PaymentPlanId);

                var person = await DbContext.People
                    .SingleOrDefaultAsync(o => o.PersonId == PersonId);

                Agreement Agreement = new()
                {
                    PropertyId = AgreementRequest.PropertyId,
                    PersonId = AgreementRequest.PersonId,
                    PaymentPlanId = AgreementRequest.PaymentPlanId,
                    PaymentDay = AgreementRequest.PaymentDay,
                    BalancePaid = 0,
                    BalancePayable = property.Price * (100 + paymentPlan.Interest) / 100
                };

                await DbContext.Agreements.AddAsync(Agreement);
                await DbContext.SaveChangesAsync();

                response.Code = ResponseCode.Ok;
                response.Data = Agreement;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }
    }
}
