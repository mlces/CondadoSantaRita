using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public partial class Token
    {
        public Guid TokenId { get; set; }
        public int TokenTypeId { get; set; }
        public string Data { get; set; } = null!;
        public DateTime Expires { get; set; }
        public int PersonId { get; set; }
        public bool Disabled { get; set; }
        public DateTime RegistrationDate { get; set; }

        public virtual User Person { get; set; } = null!;
        public virtual TokenType TokenType { get; set; } = null!;
    }
}
