namespace Core.Entities
{
    public partial class Contract
    {
        public Contract()
        {
            Payments = new HashSet<Payment>();
        }

        public int ContractId { get; set; }
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
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
