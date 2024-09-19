using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.Inventories
{
    public partial class InventoryCreate
    {
        private Inventory inventory =new() { Date = DateTime.Now };
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        public async Task OnDateChange(DateTime? date)
        {
            await Task.Delay(1);

            if(date == null)
            {
                return;
            }
            inventory.Date=(DateTime)date;
        }
        private async Task SaveInventoryAsync()
        {
            if(string.IsNullOrEmpty(inventory.Name))
            {
                Snackbar.Add("You must enter a name for the inventory.", Severity.Error);
            }
            if(string.IsNullOrEmpty(inventory.Description))
            {
                Snackbar.Add("You must enter a description to the inventory.", Severity.Error);
            }
            var parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to create this new inventory?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmDialog>("Confirmation", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            var responseHttp = await Repository.PostAsync<Inventory>("/api/inventories", inventory);
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            Snackbar.Add("Inventory created.", Severity.Success);
            NavigationManager.NavigateTo("/inventories");
        }
    }
}
