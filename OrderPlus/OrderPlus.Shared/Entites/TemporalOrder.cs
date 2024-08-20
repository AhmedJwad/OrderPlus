using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class TemporalOrder
    {
        public int Id { get; set; }
        public User? User { get; set; }
        public string? UserId { get; set; }
        public Product? Product { get; set; }
        public int ProductId { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float Quantity { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string? Remarks { get; set; }

        public decimal Value => Product == null ? 0 : Product.Price * (decimal)Quantity;
    }
}
