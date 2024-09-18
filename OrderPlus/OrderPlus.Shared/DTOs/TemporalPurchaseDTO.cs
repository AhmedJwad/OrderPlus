using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.DTOs
{
    public class TemporalPurchaseDTO
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Date")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Field {0} is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Field {0} is required.")]
        public int SupplierId { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float Quantity { get; set; } = 1;

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Cost")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Cost { get; set; }

        public string RemarksGeneral { get; set; } = string.Empty;

        public string RemarksDetail { get; set; } = string.Empty;
    }
}
