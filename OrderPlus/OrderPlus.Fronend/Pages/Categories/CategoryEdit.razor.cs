using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.Entites;
using System.Net;

namespace OrderPlus.Fronend.Pages.Categories
{
    [Authorize(Roles = "Admin")]
    public partial class CategoryEdit
    {
        private Category? category;
        private FormWithName<Category>? categoryForm;
        private bool loading;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [EditorRequired, Parameter] public int Id { get; set; }
        [CascadingParameter] private MudDialogInstance mudDialog { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            await LoadCategorieAsync();
        }

        private async Task LoadCategorieAsync()
        {
            loading = true;
            var responseHttp = await repository.GetAsync<Category>($"/api/categories/{Id}");
            loading = false;

            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    navigationManager.NavigateTo("/categories");
                }
                else
                {
                    var messsage = await responseHttp.GetErrorMessageAsync();
                    snackbar.Add(messsage, Severity.Error);
                }
            }
            else
            {
                category = responseHttp.Response;
            }
        }
        private async Task EditAsync()
        {
            var responseHttp = await repository.PutAsync("/api/categories", category);
            if (responseHttp.Error)
            {
                mudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                var parameters = new DialogParameters
                {
                    { "Message", message }
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
                dialogService.Show<GenericDialog>("Error", parameters, options);
                return;
            }

            mudDialog.Close(DialogResult.Ok(true));
            categoryForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/categories");
            snackbar.Add("Changes saved successfully.", Severity.Success);
        }

        private void Return()
        {
            mudDialog.Close(DialogResult.Cancel());
            categoryForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/categories");
        }
    }
}
