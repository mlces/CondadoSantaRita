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
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ContractsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult> Contracts()
        {
            var response = new Response<List<Contract>>();
            try
            {
                if (!User.TokenIsAccess())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var contracts = await _context.Contracts
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

                var contract = await _context.Contracts
                    .Include(o => o.Property)
                    .Include(o => o.PaymentPlan)
                    .Where(o => o.PersonId == (personIdToken != default ? personIdToken : o.PersonId))
                    .SingleOrDefaultAsync(o => o.ContractId == contractId);

                if (contract == null)
                {
                    if (!rols.Contains(Rol.Administrador.Name))
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

                var payments = await _context.Payments
                    .Include(o => o.Receiver)
                    .Include(o => o.Payer)
                    .Where(o => o.ContractId == contractId)
                    .Where(o => o.Contract.PersonId == (personIdToken != default ? personIdToken : o.Contract.PersonId))
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
    }
}
