using System.ComponentModel.DataAnnotations;

namespace Core.Models.Request
{
    public class AgreementRequest
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required]
        public int PaymentPlanId { get; set; }

        [Required]
        [Range(1, 31)]
        public int PaymentDay { get; set; }
    }
}
