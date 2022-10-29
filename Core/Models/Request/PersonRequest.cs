using System.ComponentModel.DataAnnotations;

namespace Core.Models.Request
{
    public class PersonRequest
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [EmailAddress]
        public string? EmailAddress { get; set; }
        [DataType(DataType.PhoneNumber)]
        [MaxLength(8)]
        public string? PhoneNumber { get; set; }
        [MaxLength(13)]
        public string? NationalId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Required]
        public int CityId { get; set; }
        [Required]
        public string? AddressName { get; set; }
    }
}
