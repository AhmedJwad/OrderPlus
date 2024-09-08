using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.Cart
{

    [Authorize(Roles = "Admin, User")]
    public partial class OrdersIndex
    {
        public List<Order>? Orders { get; set; }
        private MudTable<Order>? table = new();
        private readonly int[] pageSizeOptions = [10, 25, 50, 5, int.MaxValue];
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/orders";
        private string infoFormat = "{first_item}-{last_item} of {all_items}";
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;

        protected async override Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadTotalRecords()
        {
            loading=true;
            var url = $"{baseUrl}/recordsnumber?page=1&recordsnumber={int.MaxValue}";
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
        private async Task<TableData<Order>> LoadListAsync(TableState state)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}?page={page}&recordsnumber={pageSize}";

            var responseHttp = await repository.GetAsync<List<Order>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return new TableData<Order> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<Order> { Items = [], TotalItems = 0 };
            }
            return new TableData<Order>
            {
                Items = responseHttp.Response,
                TotalItems = totalRecords
            };
        }
    }
}
    