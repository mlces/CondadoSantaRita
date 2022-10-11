namespace Core.Entities
{
    public partial class Address
    {
        public int PersonId { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; } = null!;

        public virtual City City { get; set; } = null!;
        public virtual Person Person { get; set; } = null!;
    }
}
