using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;

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

        /// <summary>
        /// initialize the AddEditCompanyViewModel with the database, navigation, and alert services.
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="navigationService"></param>
        /// <param name="alertService"></param>
        public AddEditCompanyViewModel(IDatabaseService databaseService, INavigationService navigationService, IAlertService alertService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _alertService = alertService;
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
                Company = await _databaseService.GetCompanyAsync(value);
            }
            else
            {
                Title = "Add New Company";
                Company = new Company();
            }
        }

        /// <summary>
        /// save the company details to the database.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveCompanyAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Company.Name) || string.IsNullOrWhiteSpace(Company.AddressLineOne))
            {
                await _alertService.DisplayAlert("Error", "Company Name and Address Line 1 are required.", "OK");
                return;
            }

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
    }
}
