using DialogService;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Services;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Enums;
using IDialogService = MudBlazor.IDialogService;

namespace OrderPlus.Fronend.Pages.Auth
{
    public partial class Register
    {
        private UserDTO userDTO = new();
        private List<Country>? countries;
        private List<State>? states;
        private List<City>? cities;
        private bool loading;
        private string? imageUrl;
        private string? titleLabel;

        private Country? selectedCountry = new();
        private State? selectedState = new();
        private City? selectedCity = new();

        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ILoginService loginService { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Parameter , SupplyParameterFromQuery] public bool isAdmin { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await LoadCountriesAsync();
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            titleLabel = isAdmin ? "Administrator Registration" : "User Registration";
        }
        public void ImageSelected(string imageBase64)
        {
            userDTO.Photo = imageBase64;
            imageUrl = null;
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
        private async Task LoadStatesAsyn (int countryId)
        {
            var responseHttp = await repository.GetAsync<List<State>>($"/api/State/combo/{countryId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }
            states=responseHttp.Response;
        }
        private async Task LoadCitiesAsyn(int stateId)
        {
            var responseHttp = await repository.GetAsync<List<City>>($"/api/Cities/combo/{stateId}");
            if(responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }
           cities= responseHttp.Response;
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
            userDTO.CityId = city.Id;
        }

        private async Task<IEnumerable<Country>> SearchCountries(string searchText)
        {
            await Task.Delay(5);
            if(string.IsNullOrWhiteSpace(searchText))
            {
                return countries!;
            }
            return countries!.Where(x => x.Name.Contains(searchText,
                StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<State>> SearchStates(string searchText)
        {
            await Task.Delay(5);
            if(string.IsNullOrWhiteSpace(searchText))
            {
                return states!;
            }
            return states!.Where(x=>x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
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
        private void ReturnAction()
        {
            navigationManager.NavigateTo("/");
        }
        private async Task CreateUserAsync()
        {
            userDTO.UserType = UserType.User;
            userDTO.UserName = userDTO.Email;
            if(isAdmin)
            {
                userDTO.UserType = UserType.Admin;
            }
            loading = true;
            var responseHttp = await repository.PostAsync<UserDTO>("/api/accounts/CreateUser", userDTO);
            loading = false;
            if(responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }
            snackbar.Add("Your account has been created successfully. An email has been sent to you with instructions on how to activate your account.", Severity.Success);
            navigationManager.NavigateTo("/");
        }
    }
}
