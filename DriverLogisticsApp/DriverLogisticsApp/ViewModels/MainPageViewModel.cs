using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace DriverLogisticsApp.ViewModels
{
    // disable annoying warnings that are not relevant to this project
    #pragma warning disable MVVMTK0034
    #pragma warning disable MVVMTK0045

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

        /// <summary>
        /// initialize the view model for the main page
        /// </summary>
        /// <param name="databaseService"></param>
        public MainPageViewModel(IDatabaseService databaseService, INavigationService navigationService, IOnboardingService onboardingService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _onboardingService = onboardingService;
            _loads = new ObservableCollection<Load>();
            _allLoads = new List<Load>();
            OnAppearingCommand = new AsyncRelayCommand(OnAppearing);
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

        /// <summary>
        /// get all loads from the database and store them in a list for filtering
        /// </summary>
        private void FilterLoads()
        {
            IEnumerable<Load> filteredLoads;

            // check if there is any search text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                // search all loads in all statuses
                filteredLoads = _allLoads.Where(load =>
                    load.LoadNumber.ToLower().Contains(SearchText.ToLower()) ||
                    load.ShipperName.ToLower().Contains(SearchText.ToLower()));
            }
            else
            {
                // search only active loads (not cancelled)
                filteredLoads = _allLoads.Where(l =>
                    !l.IsCancelled &&
                    (l.Status == "Planned" || l.Status == "In Progress" || l.Status == "Completed"));
            }

            // clear the existing loads and add the filtered loads
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

        /// <summary>
        /// load settlement report page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToSettlementReportAsync()
        {
            await _navigationService.NavigateToAsync(nameof(Views.SettlementReportPage));
        }

        private async Task OnAppearing()
        {
            await GetLoadsAsync();
            await _onboardingService.StartOnboardingIfNeeded();
        }

        /// <summary>
        /// used to calculate KPIs such as total revenue, total expenses, and net profit
        /// </summary>
        /// <returns></returns>
        private async Task CalculateKpisAsync()
        {
            var allExpenses = await _databaseService.GetExpensesForLoadAsync(0);

            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // filter loads by month that weren't cancelled
            var monthlyLoads = _allLoads.Where(l =>
                !l.IsCancelled &&
                l.Status != "Cancelled" &&
                l.DeliveryDate.Date >= startOfMonth &&
                l.DeliveryDate.Date <= endOfMonth);

            // calculate actual revenue from completed and invoiced loads
            ActualRevenue = monthlyLoads
                .Where(l => l.Status == "Completed" || l.Status == "Invoiced")
                .Sum(l => l.FreightRate);

            // calculate potential revenue from planned and in-progress loads
            PotentialRevenue = monthlyLoads
                .Where(l => l.Status == "Planned" || l.Status == "In Progress")
                .Sum(l => l.FreightRate);

            // calculate total expenses for the month
            TotalExpenses = allExpenses
                .Where(e => e.Date.Date >= startOfMonth && e.Date.Date <= endOfMonth)
                .Sum(e => e.Amount);

            // calculate net profit
            NetProfit = ActualRevenue - TotalExpenses;
        }
    }
}