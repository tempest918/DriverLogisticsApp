using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;

namespace DriverLogisticsApp.ViewModels
{
    [QueryProperty(nameof(CompanyId), "CompanyId")]
    public partial class AddEditCompanyViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;

        [ObservableProperty]
        private Company _company = new();

        [ObservableProperty]
        private int _companyId;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private bool _isBusy;

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

        private readonly AddressDataService _addressDataService;
        public ObservableCollection<string> Countries { get; }
        public ObservableCollection<string> StatesProvinces { get; } = new();

        [ObservableProperty]
        private string _selectedCountry;



        /// <summary>
        /// initialize the AddEditCompanyViewModel with the database, navigation, and alert services.
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="navigationService"></param>
        /// <param name="alertService"></param>
        public AddEditCompanyViewModel(IDatabaseService databaseService, INavigationService navigationService, IAlertService alertService, AddressDataService addressDataService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _alertService = alertService;
            _addressDataService = addressDataService;

            Countries = new ObservableCollection<string>(_addressDataService.GetCountries());
            SelectedCountry = "USA";
        }

        /// <summary>
        /// monitor changes to the CompanyId property and load the company details if editing an existing company.
        /// </summary>
        /// <param name="value"></param>
        async partial void OnCompanyIdChanged(int value)
        {
            if (value > 0)
            {
                Title = "Edit Company";
                var company = await _databaseService.GetCompanyAsync(value);
                Company = company;
                SelectedCountry = company.Country;
            }
            else
            {
                Title = "Add New Company";
                Company = new Company();
                SelectedCountry = "USA";
            }
        }

        /// <summary>
        /// save the company details to the database.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveCompanyAsync()
        {
            // First, validate the form and set the visibility of error messages
            if (!ValidateForm())
            {
                await _alertService.DisplayAlert("Error", "Please fill in all required fields.", "OK");
                return;
            }

            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await _databaseService.SaveCompanyAsync(Company);
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", $"Failed to save company: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// handle changes to the selected country and update the states/provinces list accordingly.
        /// </summary>
        /// <param name="value"></param>
        partial void OnSelectedCountryChanged(string value)
        {
            if (Company is null) return;

            Company.Country = value;
            var originalState = Company.State;
            StatesProvinces.Clear();

            var states = _addressDataService.GetStatesProvincesForCountry(value);
            foreach (var state in states)
            {
                StatesProvinces.Add(state);
            }

            if (!string.IsNullOrWhiteSpace(originalState))
            {
                Company.State = StatesProvinces.FirstOrDefault(s => s == originalState);
            }
            else
            {
                Company.State = null;
            }
        }


        /// <summary>
        /// helper to validate the form
        /// </summary>
        /// <returns></returns>
        private bool ValidateForm()
        {
            CompanyNameErrorVisible = string.IsNullOrWhiteSpace(Company.Name);
            AddressLineOneErrorVisible = string.IsNullOrWhiteSpace(Company.AddressLineOne);
            CityErrorVisible = string.IsNullOrWhiteSpace(Company.City);
            StateErrorVisible = string.IsNullOrWhiteSpace(Company.State) || Company.State.Length != 2;
            ZipCodeErrorVisible = string.IsNullOrWhiteSpace(Company.ZipCode) || Company.ZipCode.Length < 5;
            CountryErrorVisible = string.IsNullOrWhiteSpace(Company.Country);

            return !(CompanyNameErrorVisible || AddressLineOneErrorVisible || CityErrorVisible ||
                     StateErrorVisible || ZipCodeErrorVisible || CountryErrorVisible);
        }

    }
}
