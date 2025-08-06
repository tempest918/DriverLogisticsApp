using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;

namespace DriverLogisticsApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;

        private List<Load> _allLoads;

        [ObservableProperty]
        private ObservableCollection<Load> _loads;

        [ObservableProperty]
        private string _searchText;

        /// <summary>
        /// initialize the view model for the main page
        /// </summary>
        /// <param name="databaseService"></param>
        public MainPageViewModel(IDatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _loads = new ObservableCollection<Load>();
            _allLoads = new List<Load>();
        }

        /// <summary>
        /// monitor search text, used to filter loads
        /// </summary>
        /// <param name="value"></param>
        partial void OnSearchTextChanged(string value)
        {
            FilterLoads();
        }

        /// <summary>
        /// get loads by calling the filter loads method
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GetLoadsAsync()
        {
            _allLoads = await _databaseService.GetLoadsAsync();

            FilterLoads();
        }

        /// <summary>
        /// get all loads from the database and store them in a list for filtering
        /// </summary>
        private void FilterLoads()
        {
            // if search text is empty, return all loads
            var filteredLoads = string.IsNullOrWhiteSpace(SearchText)
                ? _allLoads
                // otherwise filter loads based on search text, look in LoadNumber, ShipperName
                : _allLoads.Where(load =>
                    load.LoadNumber.ToLower().Contains(SearchText.ToLower()) ||
                    load.ShipperName.ToLower().Contains(SearchText.ToLower()));

            // clear the current loads collection and add the filtered loads
            Loads.Clear();
            foreach (var load in filteredLoads)
            {
                Loads.Add(load);
            }
        }

        /// <summary>
        /// navigate to add load page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToAddLoadAsync()
        {
            await _navigationService.NavigateToAsync("AddLoadPage");
        }

        /// <summary>
        /// load details page for the selected load
        /// </summary>
        /// <param name="load"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToDetailsAsync(Load load)
        {
            if (load is null)
                return;

            // pass load ID as a query parameter to the LoadDetailsPage
            await _navigationService.NavigateToAsync(nameof(Views.LoadDetailsPage), new Dictionary<string, object>
            {
                { "LoadId", load.Id }
            });
        }
    }
}