namespace Core.Entities
{
    public partial class Property
    {
        public Property()
        {
            Contracts = new HashSet<Contract>();
        }

        public int PropertyId { get; set; }
        public string Code { get; set; } = null!;
        public decimal Price { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
