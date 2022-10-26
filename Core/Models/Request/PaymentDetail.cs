using System.ComponentModel.DataAnnotations;

namespace Core.Models.Request
{
    public class PaymentDetailRequest
    {
        public int PaymentMethodId { get; set; }

        [Range(0, 10)]
        public int BankId { get; set; } = 0;

        public string? ReferenceNumber { get; set; }

        [Range(0, 10000)]
        public decimal Amount { get; set; } = 0;
    }
}
