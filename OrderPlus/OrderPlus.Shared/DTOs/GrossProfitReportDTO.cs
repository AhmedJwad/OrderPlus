using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.DTOs
{
    public class GrossProfitReportDTO
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public User? User { get; set; }

        public OrderType OrderType { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public Product? Product { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public float Quantity { get; set; }

        public decimal Value => (decimal)Quantity * Price;

        public decimal Profit => (decimal)Quantity * (Price - Product!.Cost);
    }
}
