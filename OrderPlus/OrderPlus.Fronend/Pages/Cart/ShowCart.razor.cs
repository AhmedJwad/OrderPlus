using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Enums;
using OrderPlus.Shared.Responses;
using System.Net.Mail;

namespace OrderPlus.Fronend.Pages.Cart
{
    [Authorize(Roles = "Admin, User")]
    public partial class ShowCart
    {
        public List<TemporalOrder>? TemporalOrders { get; set; }   
        private MudTable<TemporalOrder> table = new();
        private float sumQuantity;
        private decimal sumValue;
        private const string baseUrl = "api/temporalOrders";
        private Bank selectedBank = new();
        private List<Bank>? banks;
        private string email = null!;
        private bool loading;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        public OrderDTO OrderDTO { get; set; } = new();
        private int selectedPaymeontOption { get; set; }

        [Parameter] public int TemporalOrderId { get; set; }
        private TemporalOrderDTO? temporalOrderDTO;
        private TemporalOrder? temporalOrder;
        protected override async Task OnInitializedAsync()
        {
            await LoadBanksAsync();
           
        }

        

        private async Task LoadBanksAsync()
        {
            var responseHttp = await repository.GetAsync<List<Bank>>($"/api/banks/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message!, Severity.Error);
                return;
            }
            banks = responseHttp.Response;
        }

        private void BankChanged(Bank supplier)
        {
            selectedBank = supplier;
        }

        private async Task<IEnumerable<Bank>> SearchBankAsync(string searchText)
        {
            await Task.Delay(5);
            if(string.IsNullOrEmpty(searchText))
            {
                return banks!;
            }
            return banks!
                .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<TableData<TemporalOrder>> LoadListAsync(TableState state)
        {
            var url = $"{baseUrl}/my";
            var responseHttp = await repository.GetAsync<List<TemporalOrder>>(url);

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return new TableData<TemporalOrder> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<TemporalOrder> { Items = [], TotalItems = 0 };
            }
            sumQuantity = responseHttp.Response.Sum(x => x.Quantity);
            sumValue = responseHttp.Response.Sum(x => x.Value);
            await InvokeAsync(StateHasChanged);

            return new TableData<TemporalOrder>
            {
                Items = responseHttp.Response,
                TotalItems = responseHttp.Response.Count
            };
        }

        private async Task DeleteAsync(int temporalOrderId)
        {
            var parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to delete the record??" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = dialogService.Show<ConfirmDialog>("Confirmation", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            var responseHttp = await repository.DeleteAsync<TemporalOrder>($"api/temporalOrders/{temporalOrderId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    navigationManager.NavigateTo("/");
                    return;
                }

                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            await table.ReloadServerData();
            snackbar.Add("Product removed from shopping cart.", Severity.Success);
        }
        private async Task IncreaseQuantity(int temporalOrderId)
        {
            loading = true;
            var httpResponse = await repository.GetAsync<TemporalOrder>($"/api/temporalOrders/{temporalOrderId}");

           if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }
            temporalOrder = httpResponse.Response!;

            loading = false;
            temporalOrderDTO = new TemporalOrderDTO
            {
                Id = temporalOrder.Id,
                ProductId = temporalOrder.ProductId,
                Remarks = temporalOrder.Remarks!,
                Quantity = temporalOrder.Quantity
            };
           temporalOrderDTO.Quantity++;          
          
            await UpdateOrderAsync();
        }

        private async Task DecreaseQuantity(int temporalOrderId)
        {
            loading = true;
            var httpResponse = await repository.GetAsync<TemporalOrder>($"/api/temporalOrders/{temporalOrderId}");

            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }
            temporalOrder = httpResponse.Response!;

            loading = false;
            temporalOrderDTO = new TemporalOrderDTO
            {
                Id = temporalOrder.Id,
                ProductId = temporalOrder.ProductId,
                Remarks = temporalOrder.Remarks!,
                Quantity = temporalOrder.Quantity
            };
            temporalOrderDTO.Quantity--;
            await UpdateOrderAsync();
        }

        private async Task UpdateOrderAsync()
        {
            var httpResponse = await repository.PutAsync("/api/temporalOrders/full", temporalOrderDTO);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }
            
            snackbar.Add("Modified product in the shopping cart.", Severity.Success);          
            await table.ReloadServerData();          

        }
        private  async Task ConfirmOrderAsync()
        {
            if(selectedPaymeontOption==1)
            {
                if(selectedBank.Id==0)
                {
                    snackbar.Add("You must select a bank.", Severity.Error);
                    return;
                }
                if(string.IsNullOrEmpty(email) || !IsValidEmail(email))
                {
                    snackbar.Add("You must enter a valid email.", Severity.Error);
                    return;
                }
            }
            var parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to confirm the order?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = dialogService.Show<ConfirmDialog>("Confirmation", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }
            if(selectedPaymeontOption==1)
            {
                loading = true;
                await InvokeAsync(StateHasChanged);
                var paymentDTO = new PaymentDTO
                {
                    BankId=selectedBank.Id,
                    Email=email,
                    Value=sumValue,
                };
                var httpResponse = await repository.PostAsync<PaymentDTO, ActionResponse<string>>("/api/payments", paymentDTO);
                var response = httpResponse.Response;
                loading = false;
                if(!response!.WasSuccess)
                {
                    snackbar.Add(response.Message, Severity.Error);
                    return;
                }
                snackbar.Add(response.Message, Severity.Success);
                OrderDTO.Email = email;
                OrderDTO.Value=sumValue;
                OrderDTO.Reference = response.Result!;
                OrderDTO.BankId=selectedBank.Id;

            }    
            if(selectedPaymeontOption==0)
            {
                OrderDTO.OrderType = OrderType.PaymentAgainstDelivery;
                OrderDTO.Email = "none@none.com";
                OrderDTO.Reference = "NA";
            }
            else
            {
                OrderDTO.OrderType = OrderType.PayOnLine;
            }
            var httpActionResponse = await repository.PostAsync("/api/orders", OrderDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            navigationManager.NavigateTo("/Cart/OrderConfirmed");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {

                return false;
            }
        }
    }
}
