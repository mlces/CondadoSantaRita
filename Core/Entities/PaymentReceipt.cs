namespace Core.Entities
{
    public partial class PaymentReceipt
    {
        public int PaymentId { get; set; }
        public byte[] Data { get; set; } = null!;

        public virtual Payment Payment { get; set; } = null!;
    }
}
