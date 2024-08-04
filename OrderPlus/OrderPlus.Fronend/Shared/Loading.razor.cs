using Microsoft.AspNetCore.Components;

namespace OrderPlus.Fronend.Shared
{
    public partial class Loading
    {
        [Parameter] public string Label { get; set; } = "Please wait...";
    }
}
