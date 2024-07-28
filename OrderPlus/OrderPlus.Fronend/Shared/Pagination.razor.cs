using Microsoft.AspNetCore.Components;

namespace OrderPlus.Fronend.Shared
{
    public partial class Pagination
    {
        private List<PageModel> links=[];
        private List<OptionModel> options=[];
        private int selectedOptionValue = 10;

        [Parameter] public int CurrentPage { get; set; } = 1;
        [Parameter] public int Radio { get; set; } = 10;

        [Parameter] public EventCallback<int> RecordsNumber { get; set; }
        [Parameter]public EventCallback<int> SelectedPage { get; set; }
        [Parameter] public int TotalPages { get; set; }
        [Parameter] public bool IsHome { get; set; } = false;




        private class OptionModel
        {
            public string Name { get; set; } = null!;
            public int Value { get; set; }
        }

        private class PageModel
        {
            public bool Active { get; set; } = false;
            public bool Enable { get; set; } = true;
            public int Page { get; set; }
            public string Text { get; set; } = null!;
        }
    }
}
