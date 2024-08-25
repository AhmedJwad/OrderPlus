using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public partial class ProductKardex
    {
        public List<Kardex>? Kardex { get; set; }

        private MudTable<Kardex> table = new();
        private readonly int[] pageSizeOptions = { 5, 10, 20, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/kardex";
        private string? productName;
        private string infoFormat = "{first_item}-{last_item} de {all_items}";

        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;

        [Parameter] public int ProductId { get; set; }
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadTotalRecords()
        {
            loading = true;
            var url = $"{baseUrl}/recordsnumber?page=1&recordsnumber={int.MaxValue}&id={ProductId}";
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
        private async Task<TableData<Kardex>> LoadListAsync(TableState state)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}?page={page}&recordsnumber={pageSize}&id={ProductId}";
            var responseHttp = await repository.GetAsync<List<Kardex>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return new TableData<Kardex> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<Kardex> { Items = [], TotalItems = 0 };
            }

            productName = responseHttp.Response.FirstOrDefault()!.Product!.Name;

            return new TableData<Kardex>
            {
                Items = responseHttp.Response,
                TotalItems = totalRecords
            };
        }
    }
}
