using DialogService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using OrderPlus.Fronend.Helpers;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.Products
{
    public partial class ProductForm
    {
        private EditContext editContext = null!;
        private string? imageUrl;
        private string titleLabel => IsEdit ? "Edit product" : "create product";
        private List<MultipleSelectorModel> selected { get; set; } = new();
        private List<MultipleSelectorModel> nonSelected { get; set; } = new();
        [Inject] private MudBlazor.IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Parameter, EditorRequired] public ProductDTO productDTO { get; set; } = null!;
        [Parameter, EditorRequired] public EventCallback OnValidSubmit { get; set; }
        [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }
        [Parameter, EditorRequired] public List<Category> NonSelectedCategories { get; set; } = new();
        [Parameter] public List<Category> SelectedCategories { get; set; } = new();
        [Parameter] public EventCallback AddImageAction { get; set; }
        [Parameter] public EventCallback RemoveImageAction { get; set; }
        [Parameter] public bool IsEdit { get; set; } = false;
        public bool FormPostedSuccessfully { get; set; } = false;

        protected override void OnInitialized()
        {
            editContext= new(productDTO);
            selected=SelectedCategories.Select(x=> new MultipleSelectorModel(x.Id.ToString(),x.Name)).ToList();
            nonSelected=NonSelectedCategories.Select(x=> new MultipleSelectorModel(x.Id.ToString(), x.Name)).ToList();
        }
        private void ImageSelected(string imagenBase64)
        {
            if(productDTO.ProductImages is null)
            {
                productDTO.ProductImages = [];
            }
            productDTO.ProductImages.Add(imagenBase64);
            imageUrl = null;
        }

        private async Task OnDataAnnotationsValidatedAsync()
        {
            productDTO.ProductCategoryIds = selected.Select(x => int.Parse(x.Key)).ToList();
            await OnValidSubmit.InvokeAsync();
        }
        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWasEdited = editContext.IsModified();

            if (!formWasEdited)
            {
                return;
            }

            if (FormPostedSuccessfully)
            {
                return;
            }

            var parameters = new DialogParameters
            {
                { "Message", "Do you want to leave the page and lose your changes?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
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
