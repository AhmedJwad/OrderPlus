using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public partial class ProductEdit
    {
        private ProductDTO productDTO = new()
        {
            ProductCategoryIds = [],
            ProductImages = []
        };

        private ProductForm? productForm;
        private List<Category> selectedCategories = new();
        private List<Category> nonSelectedCategories = new();
        private bool loading = true;
        private Product? product;

        [Parameter] public int ProductId { get; set; }
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;

        [CascadingParameter] private MudDialogInstance mudDialog { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadProductAsync();
            await LoadCategoriesAsync();
        }
               
        private async Task LoadProductAsync()
        {
            loading = true;
            var httpActionResponse = await repository.GetAsync<Product>($"/api/products/{ProductId}");

            if (httpActionResponse.Error)
            {
                loading = false;
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            product = httpActionResponse.Response!;
            productDTO = ToProductDTO(product);
            loading = false;
        }      

        private async Task LoadCategoriesAsync()
        {
            loading = true;
            var httpActionResponse = await repository.GetAsync<List<Category>>("/api/categories/combo");

            if (httpActionResponse.Error)
            {
                loading = false;
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            var categories = httpActionResponse.Response!;
            foreach (var category in categories!)
            {
                var found = product!.ProductCategories!.FirstOrDefault(x => x.CategoryId == category.Id);
                if (found == null)
                {
                    nonSelectedCategories.Add(category);
                }
                else
                {
                    selectedCategories.Add(category);
                }
            }
            loading = false;
        }

        private ProductDTO ToProductDTO(Product product)
        {
            return new ProductDTO
            {
                Description = product.Description,
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                Cost = product.Cost,
                DesiredProfit = product.DesiredProfit * 100,
                ProductCategoryIds = product.ProductCategories!.Select(x => x.CategoryId).ToList(),
                ProductImages = product.ProductImages!.Select(x => x.Image).ToList()
            };
        }
        private async Task SaveChangesAsync()
        {
            var httpActionResponse = await repository.PutAsync("/api/products/full", productDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            mudDialog.Close(DialogResult.Ok(true));
            productForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo($"/products");
            snackbar.Add("Product update successfully.", Severity.Success);
        }

        private void Return()
        {
            mudDialog.Close(DialogResult.Cancel());
            productForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo($"/products");
        }

        private async Task AddImageAsync()
        {
            if (productDTO.ProductImages is null || productDTO.ProductImages.Count == 0)
            {
                return;
            }

            var imageDTO = new ImageDTO
            {
                ProductId = ProductId,
                Images = productDTO.ProductImages!
            };

            var httpActionResponse = await repository.PostAsync<ImageDTO, ImageDTO>("/api/Products/addImages", imageDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            productDTO.ProductImages = httpActionResponse.Response!.Images;
            snackbar.Add("Images added successfully.", Severity.Success);
        }
        private async Task RemoveImageAsyc()
        {
            if (productDTO.ProductImages is null || productDTO.ProductImages.Count == 0)
            {
                return;
            }

            var imageDTO = new ImageDTO
            {
                ProductId = ProductId,
                Images = productDTO.ProductImages!
            };

            var httpActionResponse = await repository.PostAsync<ImageDTO, ImageDTO>("/api/products/removeLastImage", imageDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            productDTO.ProductImages = httpActionResponse.Response!.Images;
            snackbar.Add("Image deleted successfully.", Severity.Success);
        }
    }
}
