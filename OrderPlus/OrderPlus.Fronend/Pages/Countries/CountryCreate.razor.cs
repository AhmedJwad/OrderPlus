using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.Countries
{
    public partial class CountryCreate
    {
        private Country country = new();
        private FormWithName<Country>? countryForm;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private async Task CreateAsync()
        {
            var responseHttp = await repository.PostAsync("/api/Countries", country);
            if(responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }
            MudDialog.Close(DialogResult.Ok(true));
            countryForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/countries");
            Snackbar.Add("Record created successfully.", Severity.Success);
        }
        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            countryForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/countries");
        }
    }
}
