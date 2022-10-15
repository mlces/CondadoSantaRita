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
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ContractsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        [Route("Admin")]
        public async Task<ActionResult> Contract()
        {
            var response = new Response<List<Contract>>();
            try
            {
                if (User.TokenIsReset())
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
                var errorId = Configuration.LogError(ex.ToString());
                response.Code = ResponseCode.BadRequest;
                response.Message = ResponseMessage.AnErrorHasOccurredAndId(errorId);
                return Ok(response);
            }
        }

        [HttpGet]
        [Route("{contractId}")]
        public async Task<ActionResult> Contract(int contractId)
        {
            var response = new Response<Contract>();
            try
            {
                if (User.TokenIsReset())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var personId = int.Parse(User.FindFirst(ClaimTypes.Actor).Value.ToString());

                var contract = await _context.Contracts
                    .Include(o => o.Property)
                    .Include(o => o.PaymentPlan)
                    .Where(o => o.PersonId == personId)
                    .FirstOrDefaultAsync(o => o.ContractId == contractId);

                if (contract == null)
                {
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                response.Code = ResponseCode.Ok;
                response.Data = contract;
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

        [HttpGet]
        [Route("{contractId}/Admin")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> ContractAdmin(int contractId)
        {
            var response = new Response<Contract>();
            try
            {
                if (User.TokenIsReset())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var contract = await _context.Contracts
                    .Include(o => o.Property)
                    .Include(o => o.PaymentPlan)
                    .FirstOrDefaultAsync(o => o.ContractId == contractId);

                if (contract == null)
                {
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                response.Code = ResponseCode.Ok;
                response.Data = contract;
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

        [HttpGet]
        [Route("{contractId}/[action]")]
        public async Task<ActionResult> Payments(int contractId)
        {
            var response = new Response<List<Payment>>();
            try
            {
                if (User.TokenIsReset())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var personId = int.Parse(User.FindFirst(ClaimTypes.Actor).Value.ToString());

                var payments = await _context.Payments
                    .Include(o => o.Receiver)
                    .Include(o => o.Payer)
                    .Where(o => o.ContractId == contractId)
                    .Where(o => o.Contract.PersonId == personId)
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = payments;
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

        [HttpGet]
        [Route("{contractId}/Payments/Admin")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> PaymentsAdmin(int contractId)
        {
            var response = new Response<List<Payment>>();
            try
            {
                if (User.TokenIsReset())
                {
                    response.Code = ResponseCode.Unauthorized;
                    response.Message = ResponseMessage.AnErrorHasOccurred;
                    return Ok(response);
                }

                var payments = await _context.Payments
                    .Include(o => o.Receiver)
                    .Include(o => o.Payer)
                    .Where(o => o.ContractId == contractId)
                    .ToListAsync();

                response.Code = ResponseCode.Ok;
                response.Data = payments;
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
