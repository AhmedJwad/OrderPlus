using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Pages.Countries
{
    public partial class CountriesIndex
    {
       public List<Country>? countries { get; set; }
       private MudTable<Country> table = new();
       private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
       private bool loading;
       private const string baseUrl = "api/countries";
       private string infoFormat = "{first_item}-{last_item} de {all_items}";
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Parameter, SupplyParameterFromQuery]public string Filter { get; set; }=string.Empty;




             



    }
}
