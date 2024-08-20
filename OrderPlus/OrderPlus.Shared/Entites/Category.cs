using OrderPlus.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class Category : IEntityWithName
    {
        public int Id { get; set; }

        [Display(Name = "Category")]
        [MaxLength(100, ErrorMessage = "Field {0} cannot be longer than {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Name { get; set; } = null!;

        public ICollection<ProductCategory>? ProductCategories { get; set; }

        [Display(Name = "Products")]
        public int ProductCategoriesNumber => ProductCategories == null || ProductCategories.Count == 0 ? 0 : ProductCategories.Count;
    }
}
