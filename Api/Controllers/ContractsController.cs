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
    public class ContractsController : ControllerBase, IController
    {
        public int PersonId { get; set; }

        public Guid TokenId { get; set; }

        public ApplicationContext DbContext { get; set; }

        public ContractsController(ApplicationContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> Contracts()
        {
            var response = new Response<List<Contract>>();
            try
            {
                var contracts = await DbContext.Contracts
                    .Include(o => o.Property)
                    .Include(o => o.PaymentPlan)
                    .Include(o => o.Person)
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = contracts;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpGet]
        [Route("{contractId}")]
        public async Task<ActionResult> Contract(int contractId)
        {
            var response = new Response<Contract>();
            try
            {
                if (User.IsInRole(Rol.Administrador.Name))
                {
                    PersonId = default;
                }

                var contract = await DbContext.Contracts
                    .Include(o => o.Property)
                    .Include(o => o.PaymentPlan)
                    .Include(o => o.Person)
                    .Where(o => o.PersonId == (PersonId != default ? PersonId : o.PersonId))
                    .SingleOrDefaultAsync(o => o.ContractId == contractId);

                if (contract == null)
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
                response.Data = contract;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }

        [HttpGet]
        [Route("{contractId}/[action]")]
        public async Task<ActionResult> Payments(int contractId)
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
                    .Where(o => o.ContractId == contractId)
                    .Where(o => o.Contract.PersonId == (PersonId != default ? PersonId : o.Contract.PersonId))
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
        public async Task<ActionResult> Create(ContractRequest contractRequest)
        {
            var response = new Response<Contract>();
            try
            {
                var contractExist = await DbContext.Contracts
                    .Where(o => o.PropertyId == contractRequest.PropertyId)
                    .AnyAsync();

                if (contractExist)
                {
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var property = await DbContext.Properties
                    .SingleOrDefaultAsync(o => o.PropertyId == contractRequest.PropertyId);

                var paymentPlan = await DbContext.PaymentPlans
                    .SingleOrDefaultAsync(o => o.PaymentPlanId == contractRequest.PaymentPlanId);

                var person = await DbContext.People
                    .SingleOrDefaultAsync(o => o.PersonId == PersonId);

                Contract contract = new()
                {
                    PropertyId = contractRequest.PropertyId,
                    PersonId = contractRequest.PersonId,
                    PaymentPlanId = contractRequest.PaymentPlanId,
                    PaymentDay = contractRequest.PaymentDay,
                    BalancePaid = 0,
                    BalancePayable = property.Price * (100 + paymentPlan.Interest) / 100
                };

                await DbContext.Contracts.AddAsync(contract);
                await DbContext.SaveChangesAsync();

                response.Code = ResponseCode.Ok;
                response.Data = contract;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(response.GenerateError(ex));
            }
        }
    }
}
