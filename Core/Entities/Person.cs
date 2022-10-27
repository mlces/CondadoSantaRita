using System.Text.Json.Serialization;

namespace Core.Entities
{
    public partial class Person
    {
        public Person()
        {
            Contracts = new HashSet<Contract>();
            PaymentPayers = new HashSet<Payment>();
            PaymentReceivers = new HashSet<Payment>();
        }

        public int PersonId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NationalId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? Birthday { get; set; }

        public virtual Address? Address { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Payment> PaymentPayers { get; set; }
        [JsonIgnore]
        public virtual ICollection<Payment> PaymentReceivers { get; set; }
    }
}
