﻿namespace Core.Entities
{
    public partial class Bank
    {
        public Bank()
        {
            PaymentDetails = new HashSet<PaymentDetail>();
        }

        public int BankId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; }
    }
}
