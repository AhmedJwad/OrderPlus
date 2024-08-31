using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Enums;

namespace OrderPlus.Fronend.Pages.Cart
{
    [Authorize(Roles = "Admin, User")]
    public partial class OrderDetails
    {
        private Order? order;
        public List<OrderDetail>? Details { get; set; }
        private MudTable<OrderDetail> table = new();
        private const string baseUrl = "api/orders";

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Parameter] public int OrderId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            var responseHttp = await repository.GetAsync<Order>($"{baseUrl}/{OrderId}");
            if(responseHttp.Error)
            {
                if(responseHttp.HttpResponseMessage.StatusCode==System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/orders");
                    return;
                }
                var messageError=await responseHttp.GetErrorMessageAsync();
                snackbar.Add(messageError,  Severity.Error);
            }
            order = responseHttp.Response;
            Details = order!.OrderDetails!.ToList();
        }
        private async Task<TableData<OrderDetail>> LoadListAsync(TableState state)
        {
            await LoadAsync();
            return new TableData<OrderDetail>
            {
                Items = Details,
                TotalItems = Details!.Count(),
            };
        }
        private async Task CancelOrderAsync()
        {
            await ModifyTemporalOrder("Cancel", OrderStatus.Cancelled);
        }
        private async Task DispatchOrderAsync()
        {
            await ModifyTemporalOrder("Fulfill", OrderStatus.Dispatched);
        }

        private async Task SendOrderAsync()
        {
            await ModifyTemporalOrder("Send", OrderStatus.Sent);
        }

        private async Task ConfirmOrderAsync()
        {
            await ModifyTemporalOrder("Confirm", OrderStatus.Confirmed);
        }
        private async Task ModifyTemporalOrder(string message, OrderStatus orderStatus)
        {
            var parameters = new DialogParameters
            {
                { "message", $"Are you sure you want{message}the order?"}
            };
            var option = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = dialogService.Show<ConfirmDialog>("Confirmation", parameters, option);
            var result=await dialog.Result;
            if(result.Canceled)
            {
                return;
            }

            var orderDTO = new OrderDTO
            {
                Id=OrderId,
                OrderStatus= orderStatus,
                Email = "payondeliver@yopmail.com",
                Reference = "na"
            };
            var responseHttp = await repository.PutAsync("api/orders", orderDTO);
            if (responseHttp.Error)
            {
                message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            NavigationManager.NavigateTo("/orders");
        }

    }
}
