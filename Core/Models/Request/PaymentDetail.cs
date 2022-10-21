namespace Core.Models.Request
{
    public class PaymentDetailRequest
    {
        public int PaymentMethodId { get; set; }

        public int BankId { get; set; }

        public string ReferenceNumber { get; set; }

        public decimal Amount { get; set; }
    }
}
