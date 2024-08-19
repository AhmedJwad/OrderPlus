using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.DTOs;

namespace OrderPlus.Fronend.Pages.Auth
{
    public partial class ResendConfirmationEmailToken
    {
        private EmailDTO emailDTO = new();
        private bool loading;

        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance mudDialog { get; set; } = null!;
        private async Task ResendConfirmationEmailTokenAsync()
        {
            loading = true;
            var responseHttp = await repository.PostAsync("/api/accounts/ResedToken", emailDTO);
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            mudDialog.Cancel();
            navigationManager.NavigateTo("/");
            snackbar.Add("An email has been sent to you with instructions on how to activate your user.", Severity.Success);
        }
    }
}
