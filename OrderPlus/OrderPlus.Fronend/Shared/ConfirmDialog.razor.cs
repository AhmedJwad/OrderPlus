using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace OrderPlus.Fronend.Shared
{
    public partial class ConfirmDialog
    {
        [Parameter] public string Message { get; set; } = null!;
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;

        private void Accept()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
        private void Cancel()
        {
            MudDialog.Close(DialogResult.Cancel());
        }

    }
}
