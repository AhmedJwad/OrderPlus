using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace OrderPlus.Fronend.Shared
{
    public partial class InputImg
    {
        private string? imageBase64;
        [Parameter] public string Label { get; set; } = "image";
        [Parameter] public string? ImageUrl { get; set; }
        [Parameter] public EventCallback<string> ImageSelected { get; set; }

        private  async Task OnChange(InputFileChangeEventArgs e)
        {
            var images=e.GetMultipleFiles();
            foreach (var image in images)
            {
                var arrBytes = new byte[image.Size];
                await image.OpenReadStream().ReadAsync(arrBytes);
                imageBase64=Convert.ToBase64String(arrBytes);
                ImageUrl = null;
                await ImageSelected.InvokeAsync(imageBase64);
                StateHasChanged();

            }
        }
    }
}
