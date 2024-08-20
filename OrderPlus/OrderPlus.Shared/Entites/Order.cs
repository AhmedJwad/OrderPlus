using OrderPlus.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class Order
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Date")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public DateTime Date { get; set; }

        public User? User { get; set; }

        public OrderType OrderType { get; set; }

        public string? UserId { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string? Remarks { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }

        public ICollection<OrderPayment>? OrderPayments { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Number of Order")]
        public int Lines => OrderDetails == null || OrderDetails.Count == 0 ? 0 : OrderDetails.Count;

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Quantity")]
        public float Quantity => OrderDetails == null || OrderDetails.Count == 0 ? 0 : OrderDetails.Sum(sd => sd.Quantity);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Value")]
        public decimal Value => OrderDetails == null || OrderDetails.Count == 0 ? 0 : OrderDetails.Sum(sd => sd.Value);
    }
}
