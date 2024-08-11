using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace OrderPlus.Fronend.Shared
{
    public partial class GenericDialog
    {
        [Parameter] public string Message { get; set; } = null!;
        [CascadingParameter] public MudDialogInstance mudDialog { get; set; }=null!;

        private void Close()
        {
           mudDialog?.Close();
        }

    }
}
