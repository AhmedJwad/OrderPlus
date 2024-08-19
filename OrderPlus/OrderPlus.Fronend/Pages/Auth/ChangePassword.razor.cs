using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.DTOs;

namespace OrderPlus.Fronend.Pages.Auth
{
    public partial class ChangePassword
    {
        private ChangePasswordDTO changePasswordDTO = new();
        private bool loading;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private async Task ChangePasswordAsync()
        {
            loading = true;
            var responseHttp = await repository.PostAsync("/api/accounts/changePassword", changePasswordDTO);
            loading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            MudDialog.Cancel();
            navigationManager.NavigateTo("/EditUser");
            snackbar.Add("Password Changed Successfully.", Severity.Success);
        }
        private void ReturnAction()
        {
            MudDialog.Cancel();
            navigationManager.NavigateTo("/EditUser");
        }
    }
}
