using System.ComponentModel.DataAnnotations;

namespace Core.Models.Request
{
    public class PasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
