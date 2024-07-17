using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name = "Country")]
        [MaxLength(100, ErrorMessage = "The field {0} cannot be more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Name { get; set; } = null!;

    }
}
