﻿using System.ComponentModel.DataAnnotations;

namespace Core.Models.Request
{
    public class PaymentRequest
    {
        public int ContractId { get; set; }

        [Required]
        public string? Description { get; set; }

        public int PayerId { get; set; }

        public PaymentDetailRequest[]? PaymentDetails { get; set; }

    }
}
