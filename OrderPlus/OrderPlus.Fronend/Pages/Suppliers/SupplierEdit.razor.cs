using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.Entites;
using System.Net;

namespace OrderPlus.Fronend.Pages.Suppliers
{
    [Authorize(Roles = "Admin")]
    public partial class SupplierEdit
    {
        private Supplier? supplier;
        private SupplierForm? supplierForm;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [EditorRequired, Parameter] public int Id { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            var responseHttp = await Repository.GetAsync<Supplier>($"/api/suppliers/one/{Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/suppliers");
                }
                else
                {
                    var messsage = await responseHttp.GetErrorMessageAsync();
                    Snackbar.Add(messsage, Severity.Error);
                }
            }
            else
            {
                supplier = responseHttp.Response;
            }
        }
        private async Task EditAsync()
        {
          var  supplier1 = new Supplier {SupplierName=supplier!.SupplierName,Id=supplier!.Id, Address=supplier!.Address,
              ContactFirstName=supplier!.ContactFirstName, ContactLastName=supplier!.ContactLastName,
              Phone=supplier!.Phone, Email=supplier!.Email, CityId=supplier!.CityId,
              Purchases = new List<Purchase>() };
            var responseHttp = await Repository.PutAsync("/api/Suppliers", supplier1);
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                var parameters = new DialogParameters
                {
                    { "Message", message }
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
                DialogService.Show<GenericDialog>("Error", parameters, options);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            supplierForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/suppliers");
            Snackbar.Add("Changes saved successfully.", Severity.Success);
        }

        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            supplierForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/suppliers");
        }
    }
}
