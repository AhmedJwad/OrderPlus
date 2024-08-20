using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class TemporalPurchase
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

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Cost")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Cost { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string? Remarks { get; set; }

        public decimal Value => Cost * (decimal)Quantity;
    }
}
