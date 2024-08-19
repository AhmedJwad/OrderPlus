using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Services;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using System.Net;

namespace OrderPlus.Fronend.Pages.Auth
{
    public partial class EditUser
    {
        private User? user=new User {CountryCode="+964" };
        private List<Country>? countries;
        private List<State>? states;
        private List<City>? cities;
        private bool loading = true;
        private string? imageUrl;

        private Country selectedCountry = new();
        private State selectedState = new();
        private City selectedCity = new();

        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private ILoginService loginService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadUserAsyc();
            await LoadCountriesAsync();
            await LoadStatesAsyn(user!.City!.State!.Country!.Id);
            await LoadCitiesAsyn(user!.City!.State!.Id);
            selectedCountry = user!.City!.State!.Country!;
            selectedState = user.City.State;
            selectedCity = user.City;

            if (!string.IsNullOrEmpty(user!.Photo))
            {
                imageUrl =$"https://localhost:7106/{user.Photo}";
                user.Photo = null;
            }
        }

        

        private void ShowModal()
        {
            var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
            dialogService.Show<ChangePassword>("Change Password", closeOnEscapeKey);
        }
        private async Task LoadUserAsyc()
        {
            var responseHttp = await repository.GetAsync<User>("/api/accounts");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                   navigationManager.NavigateTo("/");
                    return;
                }
                var messageError = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(messageError, Severity.Error);
                return;
            }
            user = responseHttp.Response;
            loading = false;
        }
        private async Task LoadCountriesAsync()
        {
            var responseHttp = await repository.GetAsync<List<Country>>("/api/Countries/combo");
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
            states = null;
            cities = null;
            await LoadStatesAsyn(country.Id);
        }

        private async Task StateChangedAsync(State state)
        {
            selectedState = state;
            selectedCity = new City();
            cities = null;
            await LoadCitiesAsyn(state.Id);
        }

        private void CityChanged(City city)
        {
            selectedCity = city;
            user!.CityId = city.Id;
        }
        private async Task<IEnumerable<Country>> SearchCountries(string searchText)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return countries!;
            }

            return countries!
                .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
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
        private void ImageSelected(string imagenBase64)
        {
            user!.Photo = imagenBase64;
            imageUrl = null;
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
        private async Task SaveUserAsync()
        {
            var responseHttp = await repository.PutAsync<User, TokenDTO>("/api/accounts", user!);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            await loginService.LoginAsync(responseHttp.Response!.Token);
            snackbar.Add("User modified successfully.", Severity.Success);
            navigationManager.NavigateTo("/");
        }

        private void ReturnAction()
        {
            navigationManager.NavigateTo("/");
        }
    }
}
