using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Entites
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Name { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        [MaxLength(500, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Price")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Cost")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Cost { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Profit")]
        public decimal Profit => Price - Cost;

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "value of cost")]
        public decimal CostValue => Cost * (decimal)Stock;

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "value of price")]
        public decimal PriceValue => Price * (decimal)Stock;

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "% pure profit")]
        public float RealProfit => Cost == 0 ? 0 : (float)(Profit / Cost);

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "% Desired profit")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float DesiredProfit { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float Stock { get; set; }

        public ICollection<ProductCategory>? ProductCategories { get; set; }

        [Display(Name = "Categories")]
        public int ProductCategoriesNumber => ProductCategories == null || ProductCategories.Count == 0 ? 0 : ProductCategories.Count;

        public ICollection<ProductImage>? ProductImages { get; set; }

        [Display(Name = "Images")]
        public int ProductImagesNumber => ProductImages == null || ProductImages.Count == 0 ? 0 : ProductImages.Count;

        [Display(Name = "Image")]
        public string MainImage => ProductImages == null || ProductImages.Count == 0 ? string.Empty : ProductImages.FirstOrDefault()!.Image;

        public ICollection<TemporalOrder>? TemporalOrders { get; set; }

        public ICollection<TemporalPurchase>? TemporalPurchases { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }

        public ICollection<PurchaseDetail>? PurchaseDetails { get; set; }

        public ICollection<Kardex>? Kardex { get; set; }

        public ICollection<InventoryDetail>? InventoryDetails { get; set; }
    }
}
