using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.Entites;
using System.Collections.Generic;

namespace OrderPlus.Fronend.Pages.Suppliers
{
    public partial class SupplierForm
    {
        private EditContext editContext = null!;
        private List<Country>? countries;
        private List<State>? states;
        private List<City>? cities;
        private Country selectedCountry = new();
        private State selectedState = new();
        private City selectedCity = new();
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Parameter, EditorRequired] public Supplier supplier { get; set; } = null!;
        [Parameter, EditorRequired] public EventCallback OnValidSubmit { get; set; }
        [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }
        [Parameter] public bool IsEdit { get; set; } = false;
        public bool FormPostedSuccessfully { get; set; } = false;
        private string titleLabel => IsEdit ? "Edit Supplier" : "Create Supplier";


        protected override async Task OnInitializedAsync()
        {
            editContext = new(supplier);
            await LoadCountriesAsync();
            if (IsEdit)
            {
                await LoadStatesAsyn(supplier!.City!.State!.Country!.Id);
                await LoadCitiesAsyn(supplier!.City!.State!.Id);
                selectedCountry = supplier!.City!.State!.Country!;
                selectedState = supplier!.City!.State!;
                selectedCity = supplier!.City!;
            }
        }
        private async Task OnDataAnnotationsValidatedAsync()
        {
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
        private async Task LoadCountriesAsync()
        {
            var responseHttp = await repository.GetAsync<List<Country>>("/api/countries/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }
            countries = responseHttp.Response;
        }
        private async Task LoadStatesAsyn(int countryId)
        {
            var responseHttp = await repository.GetAsync<List<State>>($"/api/State/combo/{countryId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }
            states = responseHttp.Response;
        }
        private async Task LoadCitiesAsyn(int stateId)
        {
            var responseHttp = await repository.GetAsync<List<City>>($"/api/Cities/combo/{stateId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }
            cities = responseHttp.Response;
        }

        private async Task CountryChangedAsync(Country country)
        {
            selectedCountry = country;
            selectedState = new State();
            selectedCity = new City();
            states = null!;
            cities = null!;
            await LoadStatesAsyn(country.Id);
        }
        private async Task StateChangedAsync(State state)
        {
            selectedState = state;
            selectedCity = new City();
            cities = null!;
            await LoadCitiesAsyn(state.Id);
        }
        private void CityChanged(City city)
        {
            selectedCity = city;
            supplier.CityId = city.Id;
        }

        private async Task<IEnumerable<Country>> SearchCountries(string searchString)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return countries!;
            }
            return countries!.Where(x => x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<State>> SearchStates(string searchText)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return states!;
            }

            return states!
                .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IEnumerable<City>> SearchCity(string searchText)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return cities!;
            }

            return cities!
                .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }
    }
}
