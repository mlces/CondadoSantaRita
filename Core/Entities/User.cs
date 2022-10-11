using System.Text.Json.Serialization;

namespace Core.Entities
{
    public partial class User
    {
        public User()
        {
            Rols = new HashSet<Rol>();
        }

        public int PersonId { get; set; }
        public string Username { get; set; } = null!;
        [JsonIgnore]
        public string PasswordHash { get; set; } = null!;
        public DateTime RegistrationDate { get; set; }
        public bool Disabled { get; set; }

        public virtual Person Person { get; set; } = null!;

        public virtual ICollection<Rol> Rols { get; set; }
    }
}
