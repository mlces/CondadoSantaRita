namespace Core.Entities
{
    public partial class Property
    {
        public int PropertyId { get; set; }
        public string Code { get; set; } = null!;
        public decimal Price { get; set; }

        public virtual Agreement? Agreement { get; set; }
    }
}
