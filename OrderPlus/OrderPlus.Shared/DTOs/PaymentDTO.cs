using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.DTOs
{
    public class PaymentDTO
    {
        public int BankId { get; set; }
        public string Email { get; set; } = null!;
        public decimal Value { get; set; }
    }
}
