using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class Inventory
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Date")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public DateTime Date { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Name { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        [MaxLength(500, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        public string Description { get; set; } = null!;

        public string FullName => $"{Name}, {Date.ToLocalTime():yyyy/MM/dd hh:mm tt}";

        [Display(Name = "Count 1 completed")]
        public bool Count1Finish { get; set; }

        [Display(Name = "Count 2 completed")]
        public bool Count2Finish { get; set; }

        [Display(Name = "Count 3 completed")]
        public bool Count3Finish { get; set; }

        public ICollection<InventoryDetail>? InventoryDetails { get; set; }

    }
}
