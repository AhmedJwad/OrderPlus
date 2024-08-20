using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class OrderPayment
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Date")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public DateTime Date { get; set; }
        public Order? Order { get; set; }
        public int OrderId { get; set; }

        [MaxLength(100, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        [EmailAddress(ErrorMessage = "You must enter a valid email address.")]
        public string Email { get; set; } = null!;

        public Bank? Bank { get; set; }
        public int BankId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Value")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Value { get; set; }

        [Display(Name = "Reference")]
        [MaxLength(50, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        public string Reference { get; set; } = null!;
    }
}
