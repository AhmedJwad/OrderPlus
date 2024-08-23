using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.Entites;
using System.Net;

namespace OrderPlus.Fronend.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public partial class ProductsIndex
    {
        public List<Product>? Products { get; set; }
        private MudTable<Product> table = new();
        private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/products";
        private string infoFormat = "{first_item}-{last_item} de {all_items}";
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadTotalRecords()
        {
            loading = true;
            var url = $"{baseUrl}/recordsnumber?page=1&recordsnumber={int.MaxValue}";

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }
            var responseHttp = await repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return false;
            }
            totalRecords = responseHttp.Response;
            loading = false;
            return true;
        }
        private async Task<TableData<Product>> LoadListAsync(TableState state)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}?page={page}&recordsnumber={pageSize}";

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await repository.GetAsync<List<Product>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return new TableData<Product> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<Product> { Items = [], TotalItems = 0 };
            }
            return new TableData<Product>
            {
                Items = responseHttp.Response,
                TotalItems = totalRecords
            };
        }

        private async Task SetFilterValue(string value)
        {
            Filter = value;
            await LoadAsync();
            await table.ReloadServerData();
        }

        private async Task ShowModalAsync(int id = 0, bool isEdit = false)
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            IDialogReference? dialog;
            if (isEdit)
            {
                var parameters = new DialogParameters
                {
                    { "ProductId", id }
                };
                dialog = dialogService.Show<ProductEdit>("Edit Product", parameters, options);
            }
            else
            {
                dialog = dialogService.Show<ProductCreate>("Create", options);
            }

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadAsync();
                await table.ReloadServerData();
            }
        }
        //TODO: show kadex model
        private async Task DeleteAsync(Product product)
        {
            var parameters = new DialogParameters
            {
                { "Message", $"Are you sure you want to delete the product: {product.Name}?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = dialogService.Show<ConfirmDialog>("Confirmation", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            var responseHttp = await repository.DeleteAsync<Product>($"api/products/{product.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    navigationManager.NavigateTo("/products");
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    snackbar.Add(message, Severity.Error);
                }
                return;
            }
            await LoadAsync();
            await table.ReloadServerData();
            snackbar.Add("Product removed.", Severity.Success);
        }
    }
}
