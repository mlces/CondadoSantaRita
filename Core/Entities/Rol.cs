namespace Core.Entities
{
    public partial class Rol
    {
        public Rol()
        {
            People = new HashSet<User>();
        }

        public int RolId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<User> People { get; set; }
    }
}
