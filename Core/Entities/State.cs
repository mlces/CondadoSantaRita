namespace Core.Entities
{
    public partial class State
    {
        public State()
        {
            Cities = new HashSet<City>();
        }

        public int StateId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<City> Cities { get; set; }
    }
}
