using DialogService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using OrderPlus.Fronend.Pages.Auth;
using OrderPlus.Shared.Entites;
using IDialogService = MudBlazor.IDialogService;

namespace OrderPlus.Fronend.Shared
{
    public partial class AuthLinks
    {
        private string? photoUser;
       
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
        
        protected override async Task OnParametersSetAsync()
        {
            var authenticationState = await AuthenticationStateTask;
            var claims=authenticationState.User.Claims.ToList();
            var photoClaim=claims.FirstOrDefault(x=>x.Type== "Photo");
            var nameClaim= claims.FirstOrDefault(x => x.Type == "UserName");
            if(photoClaim is not null)
            {

               photoUser = $"https://localhost:7106/{photoClaim.Value}"; ;
                
            }

        }

        private void EditAction()
        {
            navigationManager.NavigateTo("/EditUser");
        }
        private void ShowModalLogIn()
        {
            var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
            dialogService.Show<Login>("",closeOnEscapeKey);
        }
        private void ShowModalLogOut()
        {
            var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
            dialogService.Show<Logout>("LogOut", closeOnEscapeKey);
        }
    }
}
