using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.Entites;
using System.Net;

namespace OrderPlus.Fronend.Pages.News
{
    public partial class NewsDetail
    {
        private NewsArticle? newsArticle;
        private bool loading = true;

        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;

        [EditorRequired, Parameter] public int Id { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            loading = true;
            var responseHttp = await repository.GetAsync<NewsArticle>($"/api/news/{Id}");
            loading = false;

            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    navigationManager.NavigateTo("/news");
                }
                else
                {
                    var messsage = await responseHttp.GetErrorMessageAsync();
                    snackbar.Add(messsage, Severity.Error);
                }
            }
            else
            {
                newsArticle = responseHttp.Response;
            }
        }

    }
}
