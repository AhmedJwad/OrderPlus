using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class NewsArticle
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        [MaxLength(100, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Title { get; set; } = null!;

        [Display(Name = "Summary")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Summary { get; set; } = null!;
        [Display(Name = "Image")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string ImageUrl { get; set; } = null!;
        public bool Active { get; set; }
    }
}
