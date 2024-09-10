using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace OrderPlus.Fronend.Shared
{
    public partial class CultureSelector
    {
        [Inject]
        public NavigationManager _navigationManager { get; set; } = null!;
        [Inject]
        public IJSRuntime _jSRuntime { get; set; } = null!;

        CultureInfo[] cultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("ar-EG")
        };

        public CultureInfo _culture
        {
            get => CultureInfo.CurrentCulture;

            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var js = (IJSInProcessRuntime)_jSRuntime;
                    js.InvokeVoid("blazorCulture.set", value.Name);
                    // Check if the selected culture is RTL (like Arabic) and set the direction
                   

                    _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
                }
            }


        }
    }
}
