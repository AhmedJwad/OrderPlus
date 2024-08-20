using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class Supplier
    {
        public int Id { get; set; }

        [Display(Name = "Supplier Name")]
        [MaxLength(50, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string SupplierName { get; set; } = null!;        

        [Display(Name = "Contact First Name")]
        [MaxLength(50, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string ContactFirstName { get; set; } = null!;

        [Display(Name = "Contact Last Name")]
        [MaxLength(50, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string ContactLastName { get; set; } = null!;

        [Display(Name = "Address")]
        [MaxLength(200, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Address { get; set; } = null!;

        public City? City { get; set; }

        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}.")]
        public int CityId { get; set; }

        [Display(Name = "Phone")]
        [MaxLength(20, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Phone { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        [EmailAddress(ErrorMessage = "You must enter a valid email address.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Full Name")]
        public string ContactFullName => $"{ContactFirstName} {ContactLastName}";

        public ICollection<Purchase>? Purchases { get; set; }
    }
}
