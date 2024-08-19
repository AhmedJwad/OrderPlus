using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.DTOs;

namespace OrderPlus.Fronend.Pages.Auth
{
    public partial class ResetPassword
    {
        private ResetPasswordDTO resetPasswordDTO = new();
        private bool loading;

        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;

        private async Task ChangePasswordAsync()
        {
            resetPasswordDTO.Token = Token;
            loading = true;
            var responseHttp = await repository.PostAsync("/api/accounts/ResetPassword", resetPasswordDTO);
            loading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            snackbar.Add("Password changed successfully, you can now log in with your new password.", Severity.Success);
            var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
            dialogService.Show<Login>("Login", closeOnEscapeKey);
        }
    }
}
