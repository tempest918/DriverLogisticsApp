using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DriverLogisticsApp.ViewModels
{
    public partial class LoadsArchiveViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        private List<Load> _allLoads;

        [ObservableProperty]
        private ObservableCollection<Load> _loads;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private DateTime _endDate;

        [ObservableProperty]
        private string _selectedStatus;

        [ObservableProperty]
        private string _searchText;

        public ObservableCollection<string> Statuses { get; }

        public LoadsArchiveViewModel(IDatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            Loads = new ObservableCollection<Load>();
            _allLoads = new List<Load>();

            var today = DateTime.Today;
            _startDate = new DateTime(today.Year, today.Month, 1);
            _endDate = _startDate.AddMonths(1).AddDays(-1);

            Statuses = new ObservableCollection<string> { "All", "Planned", "In Progress", "Completed", "Invoiced", "Cancelled" };
            _selectedStatus = "All";
        }

        partial void OnStartDateChanged(DateTime value) => FilterLoads();
        partial void OnEndDateChanged(DateTime value) => FilterLoads();
        partial void OnSelectedStatusChanged(string value) => FilterLoads();
        partial void OnSearchTextChanged(string value) => FilterLoads();

        [RelayCommand]
        private async Task LoadLoadsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                _allLoads = await _databaseService.GetLoadsAsync();
                FilterLoads();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load loads: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FilterLoads()
        {
            if (_allLoads == null) return;

            var filtered = _allLoads.AsEnumerable();

            // Date range filter
            filtered = filtered.Where(l => l.DeliveryDate.Date >= StartDate.Date && l.DeliveryDate.Date <= EndDate.Date);

            // Status filter
            if (SelectedStatus != "All")
            {
                filtered = filtered.Where(l => l.Status == SelectedStatus);
            }

            // Search text filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(l =>
                    (l.LoadNumber != null && l.LoadNumber.ToLower().Contains(SearchText.ToLower())) ||
                    (l.ShipperName != null && l.ShipperName.ToLower().Contains(SearchText.ToLower())) ||
                    (l.ConsigneeName != null && l.ConsigneeName.ToLower().Contains(SearchText.ToLower()))
                );
            }

            Loads.Clear();
            foreach (var load in filtered)
            {
                Loads.Add(load);
            }
        }

        [RelayCommand]
        private async Task GoToLoadDetailsAsync(Load load)
        {
            if (load == null) return;

            await _navigationService.NavigateToAsync(nameof(Views.LoadDetailsPage), new Dictionary<string, object>
            {
                { "LoadId", load.Id }
            });
        }
    }
}
