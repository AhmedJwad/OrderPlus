using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

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
        [Display(Name = "Price")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Cost")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Cost { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "% Desired Utility")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float DesiredProfit { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float Stock { get; set; }

        public List<int>? ProductCategoryIds { get; set; }

        public List<string>? ProductImages { get; set; }
    }
}
