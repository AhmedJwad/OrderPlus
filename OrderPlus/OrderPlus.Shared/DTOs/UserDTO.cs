using OrderPlus.Shared.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.DTOs
{
    public class UserDTO :User
    {
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Field {0} is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Field {0} must be between {2} and {1} characters.")]
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "The password and confirmation are not the same.")]
        [Display(Name = "Password confirmation")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Field {0} is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Field {0} must be between {2} and {1} characters.")]
        public string PasswordConfirm { get; set; } = null!;
    }
}
