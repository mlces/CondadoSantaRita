namespace Core.Entities
{
    public partial class Payment
    {
        public Payment()
        {
            PaymentDetails = new HashSet<PaymentDetail>();
        }

        public int PaymentId { get; set; }
        public int PropertyId { get; set; }
        public int PaymentNumber { get; set; }
        public string Description { get; set; } = null!;
        public decimal PreviousBalancePaid { get; set; }
        public decimal NewBalancePaid { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PreviousBalancePayable { get; set; }
        public decimal NewBalancePayable { get; set; }
        public int ReceiverId { get; set; }
        public int PayerId { get; set; }
        public DateTime RegistrationDate { get; set; }

        public virtual Person Payer { get; set; } = null!;
        public virtual Agreement Agreement { get; set; } = null!;
        public virtual Person Receiver { get; set; } = null!;
        public virtual PaymentReceipt? PaymentReceipt { get; set; }
        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; }
    }
}
