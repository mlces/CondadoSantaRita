namespace Core.Entities
{
    public partial class Agreement
    {
        public Agreement()
        {
            AgreementHistories = new HashSet<AgreementHistory>();
            Payments = new HashSet<Payment>();
        }

        public int PropertyId { get; set; }
        public int PersonId { get; set; }
        public int PaymentPlanId { get; set; }
        public decimal BalancePaid { get; set; }
        public decimal BalancePayable { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool Disabled { get; set; }
        public int PaymentDay { get; set; }

        public virtual PaymentPlan PaymentPlan { get; set; } = null!;
        public virtual Person Person { get; set; } = null!;
        public virtual Property Property { get; set; } = null!;
        public virtual ICollection<AgreementHistory> AgreementHistories { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
