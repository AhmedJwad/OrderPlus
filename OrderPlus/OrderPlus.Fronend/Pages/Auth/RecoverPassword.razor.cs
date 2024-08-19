using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.DTOs;

namespace OrderPlus.Fronend.Pages.Auth
{
    public partial class RecoverPassword
    {
        private EmailDTO emailDTO = new();
        private bool loading;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private async Task SendRecoverPasswordEmailTokenAsync()
        {
            loading = true;
            var responseHttp = await Repository.PostAsync("/api/Accounts/RecoverPassword", emailDTO);
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            MudDialog.Cancel();
            NavigationManager.NavigateTo("/");
            Snackbar.Add("An email has been sent to you with instructions on how to recover your password.", Severity.Success);
        }
    }
}
