using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DriverLogisticsApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        private readonly IOnboardingService _onboardingService;

        private List<Load> _allLoads;

        [ObservableProperty]
        private ObservableCollection<Load> _loads;

        [ObservableProperty]
        private string _searchText;

        // KPIs
        [ObservableProperty]
        private decimal _actualRevenue;
        [ObservableProperty]
        private decimal _potentialRevenue;
        [ObservableProperty]
        private decimal _totalExpenses;
        [ObservableProperty]
        private decimal _netProfit;

        [ObservableProperty]
        private bool _isBusy;

        public IAsyncRelayCommand OnAppearingCommand { get; }

        public MainPageViewModel(IDatabaseService databaseService, INavigationService navigationService, IOnboardingService onboardingService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _onboardingService = onboardingService;
            _loads = new ObservableCollection<Load>();
            _allLoads = new List<Load>();
            OnAppearingCommand = new AsyncRelayCommand(OnAppearing);
        }

        private async Task OnAppearing()
        {
            await GetLoadsAsync();
            await _onboardingService.StartOnboardingIfNeeded();
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterLoads();
        }

        [RelayCommand]
        private async Task GetLoadsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                _allLoads = (await _databaseService.GetLoadsAsync())
                                    .OrderBy(l => l.PickupDate)
                                    .ToList();

                FilterLoads();
                await CalculateKpisAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FilterLoads()
        {
            IEnumerable<Load> filteredLoads;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filteredLoads = _allLoads.Where(load =>
                    load.LoadNumber.ToLower().Contains(SearchText.ToLower()) ||
                    load.ShipperName.ToLower().Contains(SearchText.ToLower()));
            }
            else
            {
                filteredLoads = _allLoads.Where(l =>
                    !l.IsCancelled &&
                    (l.Status == "Planned" || l.Status == "In Progress" || l.Status == "Completed"));
            }

            Loads.Clear();
            foreach (var load in filteredLoads)
            {
                Loads.Add(load);
            }
        }

        [RelayCommand]
        private async Task GoToAddLoadAsync()
        {
            await _navigationService.NavigateToAsync("AddLoadPage");
        }

        [RelayCommand]
        private async Task GoToDetailsAsync(Load load)
        {
            if (load is null)
                return;

            await _navigationService.NavigateToAsync(nameof(Views.LoadDetailsPage), new Dictionary<string, object>
            {
                { "LoadId", load.Id }
            });
        }

        [RelayCommand]
        private async Task GoToSettlementReportAsync()
        {
            await _navigationService.NavigateToAsync(nameof(Views.SettlementReportPage));
        }

        private async Task CalculateKpisAsync()
        {
            var allExpenses = await _databaseService.GetExpensesForLoadAsync(0);

            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var monthlyLoads = _allLoads.Where(l =>
                !l.IsCancelled &&
                l.Status != "Cancelled" &&
                l.DeliveryDate.Date >= startOfMonth &&
                l.DeliveryDate.Date <= endOfMonth);

            ActualRevenue = monthlyLoads
                .Where(l => l.Status == "Completed" || l.Status == "Invoiced")
                .Sum(l => l.FreightRate);

            PotentialRevenue = monthlyLoads
                .Where(l => l.Status == "Planned" || l.Status == "In Progress")
                .Sum(l => l.FreightRate);

            TotalExpenses = allExpenses
                .Where(e => e.Date.Date >= startOfMonth && e.Date.Date <= endOfMonth)
                .Sum(e => e.Amount);

            NetProfit = ActualRevenue - TotalExpenses;
        }
    }
}