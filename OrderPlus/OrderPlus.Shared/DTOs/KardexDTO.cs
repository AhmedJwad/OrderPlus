using OrderPlus.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.DTOs
{
    public class KardexDTO
    {
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Date")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public DateTime Date { get; set; }

        public int ProductId { get; set; }

        [Display(Name = "Movement Type")]
        public KardexType KardexType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Cost")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Cost { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float Quantity { get; set; }
    }
}
