using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.DTOs
{
    public class ChangePasswordDTO
    {
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Field {0} must be between {2} and {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string CurrentPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Field {0} must be between {2} and {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string NewPassword { get; set; } = null!;

        [Compare("NewPassword", ErrorMessage = "The new password and confirmation are not the same.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Field {0} must be between {2} and {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Confirm { get; set; } = null!;
    }
}
