using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IPreferences = DriverLogisticsApp.Services.IPreferences;

namespace DriverLogisticsApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        private readonly IPreferences _preferences;

        private List<Load> _allLoads;
        private readonly List<OnboardingStep> _onboardingSteps;
        private int _currentOnboardingStep;

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

        // Onboarding Properties
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsOnboardingFirstStep))]
        [NotifyPropertyChangedFor(nameof(IsOnboardingLastStep))]
        private bool _isOnboardingVisible;

        [ObservableProperty]
        private string _onboardingTitle;

        [ObservableProperty]
        private string _onboardingDescription;

        public bool IsOnboardingFirstStep => _currentOnboardingStep == 0;
        public bool IsOnboardingLastStep => _currentOnboardingStep == _onboardingSteps.Count - 1;

        public IAsyncRelayCommand OnAppearingCommand { get; }
        public IRelayCommand NextOnboardingStepCommand { get; }
        public IRelayCommand PreviousOnboardingStepCommand { get; }
        public IRelayCommand SkipOnboardingCommand { get; }

        public MainPageViewModel(IDatabaseService databaseService, INavigationService navigationService, IPreferences preferences)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _preferences = preferences;
            _loads = new ObservableCollection<Load>();
            _allLoads = new List<Load>();

            OnAppearingCommand = new AsyncRelayCommand(OnAppearing);
            NextOnboardingStepCommand = new RelayCommand(NextOnboardingStep);
            PreviousOnboardingStepCommand = new RelayCommand(PreviousOnboardingStep);
            SkipOnboardingCommand = new RelayCommand(SkipOnboarding);

            _onboardingSteps = new List<OnboardingStep>
            {
                new OnboardingStep { Title = "Welcome to Truck Loads!", Description = "This short tour will walk you through the key features of the app." },
                new OnboardingStep { Title = "Manage Your Loads", Description = "Create, update, and track your loads from planned to completed.\r\n\r\n To add a new load, tap the 'Add' button on the top right of the main screen.\r\n\r\nFrom the Add Load screen you can add all load details as well as create companies to use as shippers and consignees." },
                new OnboardingStep { Title = "View All Expenses", Description = "You can view all of your expenses in one place in the main menu.\r\n\r\nFrom there you can see all expenses and filter them by date." },
                new OnboardingStep { Title = "Archive Old Loads", Description = "Completed and cancelled loads are automatically moved to the archive.\r\n\r\nYou can access the archive from the main menu to view details or make changes." },
                new OnboardingStep { Title = "Track Your Expenses", Description = "Log expenses for each load, including fuel, tolls, and maintenance. You can also attach receipt photos to expenses.\r\n\r\nTo add an expense, go to a load's details and tap 'Add Expense'." },
                new OnboardingStep { Title = "Generate Reports", Description = "Create professional PDF invoices and settlement reports.\r\n\r\nYou can generate invoices from the load details screen of a completed load by tapping the create invoice toolbar item. \r\n\r\nYou can generate a settlement report from the 'Reports' section of the app." },
                new OnboardingStep { Title = "Secure Your Data", Description = "Use the PIN lock feature to keep your financial data safe.\r\n\r\nYou can enable PIN lock in the 'Settings' menu.\r\n\r\nYou can also import and export data and toggle dark mode in the Settings." },
                new OnboardingStep { Title = "Get Started!", Description = "You're all set! Tap 'Done' to start using the app." }
            };
        }

        private async Task OnAppearing()
        {
            await GetLoadsAsync();
            StartOnboarding();
        }

        private void StartOnboarding()
        {
            if (_preferences.Get("OnboardingComplete", false))
            {
                return;
            }
            _currentOnboardingStep = 0;
            UpdateOnboardingStep();
            IsOnboardingVisible = true;
        }

        private void UpdateOnboardingStep()
        {
            var step = _onboardingSteps[_currentOnboardingStep];
            OnboardingTitle = step.Title;
            OnboardingDescription = step.Description;
            OnPropertyChanged(nameof(IsOnboardingFirstStep));
            OnPropertyChanged(nameof(IsOnboardingLastStep));
        }

        private void NextOnboardingStep()
        {
            if (_currentOnboardingStep < _onboardingSteps.Count - 1)
            {
                _currentOnboardingStep++;
                UpdateOnboardingStep();
            }
        }

        private void PreviousOnboardingStep()
        {
            if (_currentOnboardingStep > 0)
            {
                _currentOnboardingStep--;
                UpdateOnboardingStep();
            }
        }

        private void SkipOnboarding()
        {
            IsOnboardingVisible = false;
            _preferences.Set("OnboardingComplete", true);
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
            var showUninvoicedLoads = _preferences.Get("show_uninvoiced_loads", true);

            var activeLoads = _allLoads.Where(l =>
                !l.IsCancelled &&
                (l.Status == "Planned" || l.Status == "In Progress" || (showUninvoicedLoads && l.Status == "Completed")));

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filteredLoads = activeLoads.Where(load =>
                    load.LoadNumber.ToLower().Contains(SearchText.ToLower()) ||
                    load.ShipperName.ToLower().Contains(SearchText.ToLower()));
            }
            else
            {
                filteredLoads = activeLoads;
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

        [RelayCommand]
        private async Task GoToAllExpensesAsync()
        {
            await _navigationService.NavigateToAsync(nameof(Views.AllExpensesPage));
        }

        [RelayCommand]
        private async Task GoToLoadsArchiveAsync()
        {
            await _navigationService.NavigateToAsync(nameof(Views.LoadsArchivePage));
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