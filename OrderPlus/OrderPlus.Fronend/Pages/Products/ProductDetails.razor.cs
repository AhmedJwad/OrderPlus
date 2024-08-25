using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using MudBlazor;

namespace OrderPlus.Fronend.Pages.Products
{
    public partial class ProductDetails
    {
        private List<string>? categories;
        private List<string>? images;
        private bool loading = true;
        private Product? product;
        private bool isAuthenticated;

        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Parameter] public int ProductId { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; } = null!;
        public TemporalOrderDTO TemporalOrderDTO { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            await CheckIsAuthenticatedAsync();
        }

        private async Task CheckIsAuthenticatedAsync()
        {
            var authenticationState = await authenticationStateTask;
            isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadProductAsync();
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
            categories = product.ProductCategories!.Select(x => x.Category!.Name).ToList();
            images = product.ProductImages!.Select(x => x.Image).ToList();
            loading = false;
        }
        private async Task AddToCartAsync()
        {
            if(!isAuthenticated)
            {
                navigationManager.NavigateTo("/Login");
                snackbar.Add("You must be logged in to add products to your shopping cart.", Severity.Error);
                return;
            }
            TemporalOrderDTO.ProductId = ProductId;

            var httpActionResponse = await repository.PostAsync("/api/temporalOrders/full", TemporalOrderDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            navigationManager.NavigateTo("/");
            snackbar.Add("Product added to shopping cart.", Severity.Success);
        }
    }
}
