using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.Banks
{
    [Authorize(Roles = "Admin")]
    public partial class BankCreate
    {
        private Bank bank = new();
        private FormWithName<Bank>? bankForm;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance mudDialog { get; set; } = null!;

        private async Task CreateAsync()
        {
            var responseHttp = await repository.PostAsync("/api/banks", bank);
            if (responseHttp.Error)
            {
                mudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            mudDialog.Close(DialogResult.Ok(true));
            bankForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/banks");
            snackbar.Add("Record created successfully.", Severity.Success);
        }

        private void Return()
        {
            mudDialog.Close(DialogResult.Cancel());
            bankForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/banks");
        }
    }
}
