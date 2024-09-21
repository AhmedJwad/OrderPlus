using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Fronend.Pages.Inventories
{
    public partial class EnterCount1
    {
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/InventoryDetails";
        private readonly int[] pageSizeOptions = [5, 10, 20, int.MaxValue];
        private bool enableModifyCost = false;
        private string infoFormat = "{first_item}-{last_item} of {all_items}";
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [EditorRequired, Parameter] public int Id { get; set; }
        [Parameter ,SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
        [CascadingParameter] private MudDialogInstance mudDialog { get; set; } = null!;
        private MudTable<InventoryDetail> table = new();
        private List<InventoryDetail>? inventoryDetails { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }
        private void OnEnableModifyCostChanged(bool value)
        {
            enableModifyCost = value;
        }
        private async Task LoadAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadTotalRecords()
        {
           loading = true;
            var url = $"{baseUrl}/recordsNumberCount1?page=1&recordsnumber={int.MaxValue}&id={Id}";
            if(!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&Filter={Filter}";
            }
            var responseHttp=await repository.GetAsync<int>(url);
            if(responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                var parameter = new DialogParameters
                {
                    { "message", message }
                };
                var options=new DialogOptions { CloseButton = true , MaxWidth=MaxWidth.ExtraSmall,CloseOnEscapeKey=true};
                dialogService.Show<GenericDialog>("Error", parameter, options);
                return false;
            }
            totalRecords = responseHttp.Response;
            loading = false;
            return true;
        }
        private async Task<TableData<InventoryDetail>> LoadListAsync(TableState state)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}/Count1?page={page}&recordsnumber={pageSize}&id={Id}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await repository.GetAsync<List<InventoryDetail>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                var parameters = new DialogParameters
                {
                    { "Message", message }
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
                dialogService.Show<GenericDialog>("Error", parameters, options);
                return new TableData<InventoryDetail> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<InventoryDetail> { Items = [], TotalItems = 0 };
            }
            inventoryDetails = responseHttp.Response;
            return new TableData<InventoryDetail>
            {
                Items = inventoryDetails,
                TotalItems = totalRecords
            };
        }
            private async Task SetFilterValue(string value)
            {
                Filter = value;
                await LoadAsync();
                await table.ReloadServerData();
            }
        private void SaveCount()
        {
            if (inventoryDetails == null) return;
            var isValid = ValidateCount();
            if(!isValid.WasSuccess)
            {
                snackbar.Add(isValid.Message!, Severity.Error);
                return;
            }
            foreach (var item in inventoryDetails)
            {
                _ = SaveInventoryDetailAsync(item);
            }
        }

       

        private ActionResponse<bool> ValidateCount()
        {
            foreach (var item in inventoryDetails!)
            {
                if(item.Count1 <0 || item.Cost <0)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccess = false,
                        Message=$"The product: {item.Product!.Name}, has cost: {item.Cost}" +
                        $" and count: {item.Count1}. Negative values ​​are not allowed."                        
                    };
                }
                if(item.Count1!=0 || item.Cost<=0)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccess = true,
                        Message = $"The product: {item.Product!.Name}" +
                        $", has a cost of 0 and a quantity entered in the count of: {item.Count1}," +
                        $" you must enter a cost if you enter a count."
                    };
                }

            }
            return new ActionResponse<bool> { WasSuccess = true };
        }
        private async Task SaveInventoryDetailAsync(InventoryDetail item)
        {
            var responseHttp = await repository.PutAsync(baseUrl, item);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                var parameters = new DialogParameters
                {
                    { "Message", message }
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
                dialogService.Show<GenericDialog>("Error", parameters, options);
                return;
            }
        }
        private async Task FinishCountAsync()
        {
            var parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to end countdown #1?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = dialogService.Show<ConfirmDialog>("Confirmation", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }
            var count0 = inventoryDetails!.Count(x=>x.Count1==0);
            if(count0/inventoryDetails!.Count>0.5)
            {
                parameters = new DialogParameters
                {
                    { "Message", "There are a lot of products with zero count, are you sure you want to close this first count?" }
                };
                options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
                dialog = dialogService.Show<ConfirmDialog>("Confirmation", parameters, options);
                result = await dialog.Result;
                if (result.Canceled)
                {
                    return;
                }
              
            }
            var responseHttp = await repository.GetAsync($"/api/inventories/finishCount1/{Id}");
            if (responseHttp.Error)
            {
                mudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message!, Severity.Error);
                return;
            }

            mudDialog.Close(DialogResult.Ok(true));
            snackbar.Add("Count #1 closed, you can proceed to count #2", Severity.Success);
            navigationManager.NavigateTo("/inventories");

        }

    }

    }

