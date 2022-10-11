using System.ComponentModel.DataAnnotations;

namespace Core.Models.Request
{
    public class TokenRequest
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
