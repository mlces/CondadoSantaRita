using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
