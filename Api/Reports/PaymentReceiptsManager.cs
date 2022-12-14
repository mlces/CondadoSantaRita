using Microsoft.Reporting.NETCore;
using System.Globalization;

namespace Api.Reports
{
    public class PaymentReceiptsManager
    {
        public byte[] Generate(Payment payment, List<PaymentDetail> paymentDetails)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}Reports\\PaymentReceipt.rdlc";

            using var file = new FileStream(path, FileMode.Open);

            LocalReport localReport = new();
            localReport.LoadReportDefinition(file);

            var parametros = new ReportParameterCollection
            {
                new ("numero_recibo", payment.PaymentId.ToString()),
                new ("nombre_cliente", $"{payment.Payer.FirstName} {payment.Payer.LastName}"),
                new ("abono_letras", payment.TotalAmount.ToWords()),
                new ("concepto", $"Abono a cuenta del lote {payment.Agreement.Property.Code}"),
                new ("cuota_mes", $"Pago de cuota No. {payment.PaymentNumber}, mes de {payment.Agreement.RegistrationDate.AddMonths(payment.PaymentNumber).ToString("MMMM yyyy", CultureInfo.GetCultureInfo("es-Es"))}"),
                new ("fecha_pago", payment.RegistrationDate.ToString("f", CultureInfo.GetCultureInfo("es-Es"))),
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
