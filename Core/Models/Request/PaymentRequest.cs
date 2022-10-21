namespace Core.Models.Request
{
    public class PaymentRequest
    {
        public int ContractId { get; set; }

        public string Description { get; set; }

        public int PayerId { get; set; }

        public PaymentDetailRequest[] PaymentDetails { get; set; } = new PaymentDetailRequest[4];

    }
}
