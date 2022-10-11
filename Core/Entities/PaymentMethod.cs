namespace Core.Entities
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            PaymentDetails = new HashSet<PaymentDetail>();
        }

        public int PaymentMethodId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; }
    }
}
