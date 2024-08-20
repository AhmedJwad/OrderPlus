using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class PurchaseDetail
    {
        public int Id { get; set; }

        public Purchase? Purchase { get; set; }

        public int PurchaseId { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string? Remarks { get; set; }

        public Product? Product { get; set; }

        public int ProductId { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Name { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        [MaxLength(500, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Cost")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Cost { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Value")]
        public decimal Value => (decimal)Quantity * Cost;
    }
}
