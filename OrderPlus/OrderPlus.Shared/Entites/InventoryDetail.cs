using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class InventoryDetail
    {
        public int Id { get; set; }

        public Inventory? Inventory { get; set; }

        public int InventoryId { get; set; }

        public Product? Product { get; set; }

        public int ProductId { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float Stock { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Cost")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Cost { get; set; }

        [Display(Name = "Count 1")]
        public float Count1 { get; set; }

        [Display(Name = "Count 2")]
        public float Count2 { get; set; }

        [Display(Name = "Count 3")]
        public float Count3 { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Value")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float Adjustment { get; set; }

        public decimal AdjustmentValue => (decimal)Adjustment * Cost;
    }
}
