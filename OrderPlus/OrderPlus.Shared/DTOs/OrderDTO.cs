using OrderPlus.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string Remarks { get; set; } = string.Empty;

        public OrderType OrderType { get; set; }

        public string Email { get; set; } = null!;

        public int BankId { get; set; }

        public decimal Value { get; set; }

        public string Reference { get; set; } = null!;
    }
}
