using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.News
{
    public partial class NewsCreate
    {
        private NewsArticle newsArticle = new() { Active = true };
        private NewsForm? newsForm;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        [CascadingParameter] private MudDialogInstance mudDialog { get; set; } = null!;

        private async Task CreateAsync()
        {
            var responseHttp = await repository.PostAsync("/api/news", newsArticle);
            if (responseHttp.Error)
            {
                mudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            mudDialog.Close(DialogResult.Ok(true));
            newsForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/news");
           snackbar.Add("news created successfully.", Severity.Success);
        }
        private void Return()
        {
            mudDialog.Close(DialogResult.Cancel());
            newsForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/news");
        }
    }
}
