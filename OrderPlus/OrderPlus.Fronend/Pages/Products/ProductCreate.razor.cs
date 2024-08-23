using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using System.ComponentModel;

namespace OrderPlus.Fronend.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public partial class ProductCreate
    {
        private ProductDTO productDTO = new()
        {
            ProductCategoryIds = new List<int>(),
            ProductImages = new List<string>(),
        };
        private ProductForm? productForm;
        private List<Category> nonSelectedCategories = new();
        private bool loading  = true;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance mudDialog { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            var httpActionResponse = await repository.GetAsync<List<Category>>("/api/categories/combo");
            loading = false;

            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            nonSelectedCategories = httpActionResponse.Response!;
        }
        private async Task CreateAsync()
        {
            var httpActionResponse = await repository.PostAsync("/api/products/full", productDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            mudDialog.Close(DialogResult.Ok(true));
            productForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo($"/products");
            snackbar.Add("Product created successfully.", Severity.Success);
        }

        private void Return()
        {
            mudDialog.Close(DialogResult.Cancel());
            productForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo($"/products");
        }

    }
}
