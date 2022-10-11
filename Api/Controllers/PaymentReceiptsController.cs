using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Humanizer;
using Microsoft.Reporting.NETCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentReceiptsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PaymentReceiptsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{paymentId}")]
        public async Task<ActionResult> PaymentReceipt(int paymentId)
        {
            var response = new Response<PaymentReceipt>();
            try
            {
                var paymentReceipt = await _context.PaymentReceipts
                    .FirstOrDefaultAsync(o => o.PaymentId == paymentId);

                if (paymentReceipt == null)
                {
                    var report = await Generate(paymentId);

                    paymentReceipt = new()
                    {
                        PaymentId = paymentId,
                        Data = report
                    };

                    _context.PaymentReceipts.Add(paymentReceipt);
                    _context.SaveChanges();
                    paymentReceipt.Payment = null;
                }

                response.Code = 0;
                response.Data = paymentReceipt;
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorId = Configuration.LogError(ex.ToString());
                response.Message = $"Ha ocurrido un error, intente nuevamente o reporte el error: {errorId}.";
                return Ok(response);
            }
        }

        private async Task<byte[]> Generate(int paymentId)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}Reports\\PaymentReceipt.rdlc";

            var payment = await _context.Payments
                .Include(o => o.Payer)
                .Include(o => o.Receiver)
                .FirstOrDefaultAsync(o => o.PaymentId == paymentId);

            var paymentDetails = await _context.PaymentDetails
                .Where(o => o.PaymentId == paymentId)
                .Include(o => o.Bank)
                .ToListAsync();

            using var file = new FileStream(path, FileMode.Open);

            LocalReport localReport = new();
            localReport.LoadReportDefinition(file);

            var parametros = new ReportParameterCollection
            {
                new ("numero_recibo", payment.PaymentId.ToString()),
                new ("nombre_cliente", $"{payment.Payer.FirstName} {payment.Payer.LastName}"),
                new ("abono_letras", payment.TotalAmount.ToWords()),
                new ("concepto", payment.Description),
                new ("fecha_pago", DateTime.Now.ToString("f", CultureInfo.GetCultureInfo("es-Es"))),
                new ("abono_numeros", payment.TotalAmount.ToCurrency()),
                new ("saldo_anterior", payment.PreviousBalancePayable.ToCurrency()),
                new ("saldo_actual", payment.NewBalancePayable.ToCurrency())
            };

            foreach (var item in paymentDetails)
            {
                switch (item.PaymentMethodId)
                {
                    case 1:
                        parametros.Add(new("monto_efectivo", item.Amount.ToCurrency()));
                        break;
                    case 2:
                        parametros.Add(new("monto_transferencia", item.Amount.ToCurrency()));
                        parametros.Add(new("banco_transferencia", item.Bank.Name));
                        parametros.Add(new("referencia_transferencia", item.ReferenceNumber));
                        break;
                    case 3:
                        parametros.Add(new("monto_cheque", item.Amount.ToCurrency()));
                        parametros.Add(new("banco_cheque", item.Bank.Name));
                        parametros.Add(new("referencia_cheque", item.ReferenceNumber));
                        break;
                    case 4:
                        parametros.Add(new("monto_deposito", item.Amount.ToCurrency()));
                        parametros.Add(new("banco_deposito", item.Bank.Name));
                        parametros.Add(new("referencia_deposito", item.ReferenceNumber));
                        break;
                    default:
                        break;
                }
            }

            localReport.SetParameters(parametros);

            return localReport.Render("PDF");
        }
    }
}
