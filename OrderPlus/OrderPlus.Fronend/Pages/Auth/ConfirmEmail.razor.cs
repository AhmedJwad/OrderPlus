using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;

namespace OrderPlus.Fronend.Pages.Auth
{
    public partial class ConfirmEmail
    {
        private string? message;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string UserId { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;

        private async Task ConfirmAccountAsync()
        {
            var responseHttp=await repository.GetAsync($"/api/accounts/ConfirmEmail/?userId={UserId}&token={Token}");
            if (responseHttp.Error)
            {
                message = await responseHttp.GetErrorMessageAsync();
                NavigationManager.NavigateTo("/");
                Snackbar.Add(message, Severity.Error);
                return;
            }
            Snackbar.Add("Thank you for confirming your email, you can now log in to the system.", Severity.Success);
            var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
            DialogService.Show<Login>("Login", closeOnEscapeKey);
        }
    }
}
