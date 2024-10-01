using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Pages.News;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages
{
    public partial class NewsAndPromotions
    {
        private List<NewsArticle>? newsArticles;
        private const string baseUrl = "api/news";
        private bool loading = true;

        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
           await LoadAsync();
        }

        private async Task LoadAsync()
        {
            loading = true;
            var url = $"{baseUrl}?page=1&recordsnumber={int.MaxValue}&Id=1";
            var responseHttp = await repository.GetAsync<List<NewsArticle>>(url);
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
            }

            newsArticles = responseHttp.Response;
        }
        private static string TruncateContent(string content, int length)
        {
            if (content.Length > length)
            {
                return content.Substring(0, length) + "...";
            }
            return content;
        }

        private void ViewDetails(NewsArticle news)
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters
            {
                { "Id", news.Id }
            };
            dialogService.Show<NewsDetail>(news.Title, parameters, options);
        }
    }
}
