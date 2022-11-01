namespace Core.Entities
{
    public partial class AgreementHistory
    {
        public int HistoryId { get; set; }
        public int PropertyId { get; set; }
        public int PersonId { get; set; }
        public int PaymentPlanId { get; set; }
        public decimal BalancePaid { get; set; }
        public decimal BalancePayable { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool Disabled { get; set; }
        public int PaymentDay { get; set; }

        public virtual Agreement Property { get; set; } = null!;
    }
}
