using OrderPlus.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class Country :IEntityWithName
    {
        public int Id { get; set; }

        [Display(Name = "Country")]
        [MaxLength(100, ErrorMessage = "The field {0} cannot be more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Name { get; set; } = null!;

        public ICollection<State>? States{ get; set; }

        [Display(Name = "Departments/States")]
        public int StatesNumber=> States ==null || States.Count==0? 0 : States.Count;

    }
}
