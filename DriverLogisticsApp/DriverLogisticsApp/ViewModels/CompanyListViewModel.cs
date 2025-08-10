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

        private List<Company> _allCompanies;

        [ObservableProperty]
        private ObservableCollection<Company> _companies;

        [ObservableProperty]
        private string _searchText;

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
            _allCompanies = new List<Company>();
        }
        partial void OnSearchTextChanged(string value)
        {
            FilterCompanies();
        }

        /// <summary>
        /// get the list of companies from the database and populate the Companies collection.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GetCompaniesAsync()
        {
            _allCompanies = await _databaseService.GetCompaniesAsync();
            FilterCompanies();
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

        /// <summary>
        /// method to filter companies based on search text.
        /// </summary>
        private void FilterCompanies()
        {
            IEnumerable<Company> filtered;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                // if there is search text, search ALL companies
                filtered = _allCompanies.Where(c => c.Name.ToLower().Contains(SearchText.ToLower()));
            }
            else
            {
                // search is empty, show only ACTIVE companies
                filtered = _allCompanies.Where(c => c.IsActive);
            }

            Companies.Clear();
            foreach (var company in filtered)
            {
                Companies.Add(company);
            }
        }

    }
}
