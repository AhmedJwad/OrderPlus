using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.Suppliers
{
    [Authorize(Roles = "Admin")]
    public partial class SupplierCreate
    {
        private Supplier supplier = new();
        private SupplierForm? supplierForm;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private async Task CreateAsync()
        {
            var responseHttp = await repository.PostAsync("/api/suppliers", supplier);
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            supplierForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/suppliers");
            snackbar.Add("Rrecord created successfully.", Severity.Success);
        }
        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            supplierForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/suppliers");
        }
    }
}
