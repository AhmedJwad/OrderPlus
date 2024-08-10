using DialogService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using OrderPlus.Shared.Interfaces;
using System.Xml.Linq;

namespace OrderPlus.Fronend.Shared
{
    public partial class FormWithName<TModel> where TModel: IEntityWithName
    {
        private EditContext EditContext = null!;

        [EditorRequired, Parameter] public TModel model { get; set; } = default!;
        [EditorRequired, Parameter] public string Label { get; set; } = null!;
        [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
        [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }
        [Inject] private MudBlazor.IDialogService dialogService { get; set; }=null!;
        public bool FormPostedSuccessfully { get; set; }


        protected override void OnInitialized()
        {
            EditContext = new(model);
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWasEdited = EditContext.IsModified();
            if (!formWasEdited || FormPostedSuccessfully)
            {
                return;
            }
            var parameters = new DialogParameters
            {
                { "Message", "Do you want to leave the page and lose your changes?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog=dialogService.Show<ConfirmDialog>("Confirmation", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            context.PreventNavigation();
        }
    }
}
