using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public partial class User
    {
        public User()
        {
            Tokens = new HashSet<Token>();
            Rols = new HashSet<Rol>();
        }

        public int PersonId { get; set; }
        public string Username { get; set; } = null!;
        [NotMapped]
        public string Password { get; set; } = null!;
        [JsonIgnore]
        public string PasswordHash { get; set; } = null!;
        public DateTime RegistrationDate { get; set; }
        public bool Disabled { get; set; }
        public bool? ResetPassword { get; set; }

        public virtual Person Person { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Token> Tokens { get; set; }
        public virtual ICollection<Rol> Rols { get; set; }
    }
}
