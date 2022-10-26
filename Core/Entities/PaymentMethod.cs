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

        public static PaymentMethod Efectivo { get; private set; } = new() { PaymentMethodId = 1, Name = nameof(Efectivo) };
        public static PaymentMethod Transferencia { get; private set; } = new() { PaymentMethodId = 2, Name = nameof(Transferencia) };
        public static PaymentMethod Cheque { get; private set; } = new() { PaymentMethodId = 3, Name = nameof(Cheque) };
        public static PaymentMethod Deposito { get; private set; } = new() { PaymentMethodId = 4, Name = nameof(Deposito) };
    }
}
