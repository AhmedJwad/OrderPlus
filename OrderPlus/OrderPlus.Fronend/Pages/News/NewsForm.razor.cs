using DialogService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using System.Collections.Generic;


namespace OrderPlus.Fronend.Pages.News
{
    public partial class NewsForm
    {
        private EditContext editContext = null!;
        private string? imageUrl;
        [Inject] private MudBlazor.IDialogService dialogService {  get; set; }=null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Parameter, EditorRequired] public NewsArticle newsArticle { get; set; } = null!;
        [Parameter, EditorRequired] public EventCallback OnValidSubmit { get; set; }
        [Parameter ,EditorRequired] public EventCallback ReturnAction { get; set; }
        [Parameter] public bool isEdit { get; set; } = false;
        public bool FormPostedSuccessfully { get; set; } = false;
        private string titleLabel => isEdit ? "Edit News" : "Create News";

        protected override void OnInitialized()
        {
            base.OnInitialized();
            editContext = new(newsArticle);
        }
        private void ImageSelected(string imagenBase64)
        {
            if(!string.IsNullOrEmpty(newsArticle.ImageUrl))
            {
                imageUrl = newsArticle.ImageUrl;
            }

            newsArticle.ImageUrl = imagenBase64;
            imageUrl = null;
        }
        private async Task OnDataAnnotationsValidatedAsync()
        {
            await OnValidSubmit.InvokeAsync();
        }
        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWasEdited = editContext.IsModified();
            if(!formWasEdited)
            {
                return;
            }
            if(FormPostedSuccessfully)
            {
                return;
            }
            var parameters = new DialogParameters
            {
                {"message","Do you want to leave the page and lose your changes?" }
            };
            var options=new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = dialogService.Show<ConfirmDialog>("Confirmation", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            context.PreventNavigation();
        }
    }
}
