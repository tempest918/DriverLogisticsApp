using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;

namespace DriverLogisticsApp.ViewModels
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IAlertService _alertService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly AddressDataService _addressDataService;

        [ObservableProperty]
        private UserProfile _profile;

        [ObservableProperty]
        private string _newPin;

        [ObservableProperty]
        private string _confirmPin;

        [ObservableProperty]
        private bool _pinsDoNotMatchErrorVisible;

        [ObservableProperty]
        private bool _isAuthenticationEnabled;

        public ObservableCollection<string> Countries { get; }
        public ObservableCollection<string> StatesProvinces { get; } = new();

        [ObservableProperty]
        private string _selectedCountry;

        [ObservableProperty]
        private bool _userNameErrorVisible;
        [ObservableProperty]
        private bool _companyNameErrorVisible;
        [ObservableProperty]
        private bool _addressLineOneErrorVisible;
        [ObservableProperty]
        private bool _cityErrorVisible;
        [ObservableProperty]
        private bool _stateErrorVisible;
        [ObservableProperty]
        private bool _zipCodeErrorVisible;
        [ObservableProperty]
        private bool _countryErrorVisible;

        private bool _isInitializing = false;

        /// <summary>
        /// initializes the view model with required services
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="alertService"></param>
        public ProfilePageViewModel(IDatabaseService databaseService, IAlertService alertService, ISecureStorageService secureStorageService, AddressDataService addressDataService)
        {
            _databaseService = databaseService;
            _alertService = alertService;
            _secureStorageService = secureStorageService;
            _addressDataService = addressDataService;

            Countries = new ObservableCollection<string>(_addressDataService.GetCountries());
        }

        /// <summary>
        /// loads the user profile from the database
        /// </summary>
        /// <returns></returns>
        public async Task LoadProfileAsync()
        {
            _isInitializing = true;

            Profile = await _databaseService.GetUserProfileAsync();

            var country = !string.IsNullOrWhiteSpace(Profile.CompanyCountry) ? Profile.CompanyCountry : "USA";
            SelectedCountry = country;

            UpdateStatesForCountry(country);

            // Restore the state selection if it's valid for the country
            var originalState = Profile.CompanyState;
            if (!string.IsNullOrWhiteSpace(originalState) && StatesProvinces.Contains(originalState))
            {
                Profile.CompanyState = originalState;
            }
            else
            {
                Profile.CompanyState = null;
            }

            var savedPin = await _secureStorageService.GetAsync("user_pin");
            IsAuthenticationEnabled = !string.IsNullOrWhiteSpace(savedPin);

            _isInitializing = false;
        }

        /// <summary>
        /// enables or disables authentication based on user input
        /// </summary>
        /// <param name="value"></param>
        async partial void OnIsAuthenticationEnabledChanged(bool value)
        {
            if (!value)
            {
                bool confirmed = await _alertService.DisplayAlert("Disable Security", "Are you sure you want to disable PIN authentication? Your app will no longer be password protected.", "Yes, Disable", "Cancel");
                if (confirmed)
                {
                    _secureStorageService.Remove("user_pin");
                    await _alertService.DisplayAlert("Success", "PIN authentication has been disabled.", "OK");
                }
                else
                {
                    IsAuthenticationEnabled = true;
                }
            }
        }

        /// <summary>
        /// when the selected country changes, update the states/provinces list and reset the state selection
        /// </summary>
        /// <param name="value"></param>
        partial void OnSelectedCountryChanged(string value)
        {
            if (_isInitializing)
                return;

            UpdateStatesForCountry(value);
            // When user manually changes country, reset the state.
            Profile.CompanyState = null;
        }

        private void UpdateStatesForCountry(string country)
        {
            if (Profile is null) return;

            Profile.CompanyCountry = country;
            StatesProvinces.Clear();

            var states = _addressDataService.GetStatesProvincesForCountry(country);
            foreach (var state in states)
            {
                StatesProvinces.Add(state);
            }
        }


        /// <summary>
        /// saves the user profile after validating the input
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (!ValidateProfile())
            {
                await _alertService.DisplayAlert("Error", "Please fill in all required fields.", "OK");
                return;
            }

            await _databaseService.SaveUserProfileAsync(Profile);
            await _alertService.DisplayAlert("Success", "Your profile has been saved.", "OK");
        }

        /// <summary>
        /// checks if the new PIN matches the confirmation PIN and updates the error visibility
        /// </summary>
        /// <param name="value"></param>
        partial void OnNewPinChanged(string value)
        {
            // Check if the pins match and update the error visibility
            PinsDoNotMatchErrorVisible = !string.IsNullOrEmpty(ConfirmPin) && value != ConfirmPin;
        }

        /// <summary>
        /// checks if the confirmation PIN matches the new PIN and updates the error visibility
        /// </summary>
        /// <param name="value"></param>
        partial void OnConfirmPinChanged(string value)
        {
            // Check if the pins match and update the error visibility
            PinsDoNotMatchErrorVisible = value != NewPin;
        }

        /// <summary>
        /// sets a new PIN after validating the input
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SetPinAsync()
        {
            if (NewPin != ConfirmPin)
            {
                PinsDoNotMatchErrorVisible = true;
                await _alertService.DisplayAlert("Error", "PINs do not match. Please try again.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPin) || NewPin.Length < 4)
            {
                await _alertService.DisplayAlert("Error", "Please enter a valid 4-digit PIN.", "OK");
                return;
            }

            await _secureStorageService.SetAsync("user_pin", NewPin);

            NewPin = string.Empty;
            ConfirmPin = string.Empty;
            IsAuthenticationEnabled = true;

            await _alertService.DisplayAlert("Success", "Your PIN has been set.", "OK");
        }

        /// <summary>
        /// validates the user profile input and sets error flags accordingly
        /// </summary>
        /// <returns></returns>
        private bool ValidateProfile()
        {
            UserNameErrorVisible = string.IsNullOrWhiteSpace(Profile.UserName);
            CompanyNameErrorVisible = string.IsNullOrWhiteSpace(Profile.CompanyName);
            AddressLineOneErrorVisible = string.IsNullOrWhiteSpace(Profile.CompanyAddressLineOne);
            CityErrorVisible = string.IsNullOrWhiteSpace(Profile.CompanyCity);
            StateErrorVisible = string.IsNullOrWhiteSpace(Profile.CompanyState);
            ZipCodeErrorVisible = string.IsNullOrWhiteSpace(Profile.CompanyZipCode);
            CountryErrorVisible = string.IsNullOrWhiteSpace(Profile.CompanyCountry);

            return !(UserNameErrorVisible || CompanyNameErrorVisible || AddressLineOneErrorVisible ||
                     CityErrorVisible || StateErrorVisible || ZipCodeErrorVisible || CountryErrorVisible);
        }
    }
}
