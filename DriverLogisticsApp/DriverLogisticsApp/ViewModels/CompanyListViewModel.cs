using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;

namespace DriverLogisticsApp.ViewModels
{
    public partial class CompanyListViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Company> _companies;

        /// <summary>
        /// initialize the CompanyListViewModel with the database and navigation services.
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="navigationService"></param>
        public CompanyListViewModel(IDatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            Companies = new ObservableCollection<Company>();
        }

        /// <summary>
        /// get the list of companies from the database and populate the Companies collection.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GetCompaniesAsync()
        {
            var companiesFromDb = await _databaseService.GetCompaniesAsync();
            Companies.Clear();
            foreach (var company in companiesFromDb)
            {
                Companies.Add(company);
            }
        }

        /// <summary>
        /// goto the company details page for the selected company.
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToCompanyDetailsAsync(Company company)
        {
            var companyId = company?.Id ?? 0;

            await _navigationService.NavigateToAsync(nameof(Views.AddEditCompanyPage), new Dictionary<string, object>
            {
                { "CompanyId", companyId }
            });
        }
    }
}
