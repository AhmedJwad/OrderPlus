using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.Entites;
using System.Net;

namespace OrderPlus.Fronend.Pages.Banks
{
    [Authorize(Roles = "Admin")]
    public partial class BankEdit
    {
        private Bank? bank;
        private FormWithName<Bank>? bankForm;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        [EditorRequired, Parameter] public int Id { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            var responseHttp = await repository.GetAsync<Bank>($"/api/banks/{Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    navigationManager.NavigateTo("/banks");
                }
                else
                {
                    var messsage = await responseHttp.GetErrorMessageAsync();
                    snackbar.Add(messsage, Severity.Error);
                }
            }
            else
            {
                bank = responseHttp.Response;
            }
        }

        private async Task EditAsync()
        {
            var responseHttp = await repository.PutAsync("/api/banks", bank);
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                var parameters = new DialogParameters
                {
                    { "Message", message }
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
                dialogService.Show<GenericDialog>("Error", parameters, options);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            bankForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/banks");
            snackbar.Add("Changes saved successfully.", Severity.Success);
            
        }

        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            bankForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/banks");
        }
    }
}
