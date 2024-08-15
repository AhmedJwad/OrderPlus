using Microsoft.AspNetCore.Identity;
using OrderPlus.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class User:IdentityUser
    {
       
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last name")]
        [MaxLength(50, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Address")]
        [MaxLength(200, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Address { get; set; } = null!;

        [DefaultValue("+964")]
        [Display(Name = "Country Code")]
        [MaxLength(5, ErrorMessage = "The {0} field cannot be longer than {1} characters.")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public string CountryCode { get; set; } = null!;

        [Display(Name = "Photo")]
        public string? Photo { get; set; }

        [Display(Name = "User type")]
        public UserType UserType { get; set; }

        public City? City { get; set; }

        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}.")]
        public int CityId { get; set; }

        [Display(Name = "User")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
