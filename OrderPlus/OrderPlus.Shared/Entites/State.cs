using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class State
    {
        public int Id { get; set; }

        [Display(Name = "Department/State")]
        [MaxLength(100, ErrorMessage = "The field {0} cannot be more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Name { get; set; } = null!;

        public int CountryId { get; set; }

        public Country? Country { get; set; }

        public ICollection<City>? Cities { get; set; }

        [Display(Name=("Cities"))]
        public int CitiesNumber=>Cities==null || Cities.Count==0 ? 0 :Cities.Count;

    }
}
