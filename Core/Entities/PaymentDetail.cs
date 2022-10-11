namespace Core.Entities
{
    public partial class PaymentDetail
    {
        public int PaymentDetailId { get; set; }
        public int PaymentId { get; set; }
        public int PaymentMethodId { get; set; }
        public int? BankId { get; set; }
        public string? ReferenceNumber { get; set; }
        public decimal Amount { get; set; }

        public virtual Bank? Bank { get; set; }
        public virtual Payment Payment { get; set; } = null!;
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;
    }
}
