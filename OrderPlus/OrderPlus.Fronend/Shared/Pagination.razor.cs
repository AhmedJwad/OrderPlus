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


        protected override void OnParametersSet()
        {
            BuildPages();
            BuildOptions();
        }
        private void BuildPages()
        {
            links = [];
            var previousLinkEnable = CurrentPage != 1;
            var previousLinkPage = CurrentPage - 1;
            links.Add(new PageModel
            {
                Text = "previously",
                Page = previousLinkPage,
                Enable = previousLinkEnable
            });
            for (int i = 0; i <TotalPages; i++)
            {
                if(TotalPages <= Radio)
                {
                    links.Add(new PageModel
                    {
                        Page = i,
                        Enable = CurrentPage == i,
                        Text = $"{i}"
                    });
                }
                if (TotalPages > Radio && i <= Radio && CurrentPage <= Radio)
                {
                    links.Add(new PageModel
                    {
                        Page = i,
                        Enable = CurrentPage == i,
                        Text = $"{i}"
                    });
                }

                if (CurrentPage > Radio && i > CurrentPage - Radio && i <= CurrentPage)
                {
                    links.Add(new PageModel
                    {
                        Page = i,
                        Enable = CurrentPage == i,
                        Text = $"{i}"
                    });
                }
            }
            links.Add(new PageModel
            {
                Text = "Next",
                Page = CurrentPage != TotalPages ? CurrentPage + 1 : CurrentPage,
                Enable = CurrentPage != TotalPages
            });
        }
        private void BuildOptions()
        {
            if (IsHome)
            {
                options = [
                       new OptionModel{Value=8, Name="8"},
                        new OptionModel{Value=16, Name="16"},
                        new OptionModel{Value=32, Name="32"},
                        new OptionModel{Value=int.MaxValue, Name="All"},

                      ];
            }
            else
            {
                options = [
                       new OptionModel{Value=10, Name="10"},
                        new OptionModel{Value=25, Name="25"},
                        new OptionModel{Value=50, Name="50"},
                        new OptionModel{Value=int.MaxValue, Name="All"},

                      ];
            }
        }

        private async Task InternalRecordsNumberSelected(ChangeEventArgs e)
        {
            if (e.Value != null)
            {
                selectedOptionValue = Convert.ToInt32(e.Value.ToString());
            }
            await RecordsNumber.InvokeAsync(selectedOptionValue);
        }
        private async Task InternalSelectedPage(PageModel pageModel)
        {
            if (pageModel.Page == CurrentPage || pageModel.Page == 0)
            {
                return;
            }
            await SelectedPage.InvokeAsync(pageModel.Page);
        }

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
