namespace Core.Entities
{
    public partial class PaymentPlan
    {
        public PaymentPlan()
        {
            Contracts = new HashSet<Contract>();
        }

        public int PaymentPlanId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Interest { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
