using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Threading.Tasks;

namespace DriverLogisticsApp.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        private readonly IAlertService _alertService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly IDatabaseService _databaseService;
        private readonly IJsonImportExportService _jsonService;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _newPin;

        [ObservableProperty]
        private string _confirmPin;

        [ObservableProperty]
        private bool _pinsDoNotMatchErrorVisible;

        [ObservableProperty]
        private bool _isAuthenticationEnabled;

        [ObservableProperty]
        private bool _isDarkMode;

        public SettingsPageViewModel(IAlertService alertService, ISecureStorageService secureStorageService, IDatabaseService databaseService, IJsonImportExportService jsonService)
        {
            _alertService = alertService;
            _secureStorageService = secureStorageService;
            _databaseService = databaseService;
            _jsonService = jsonService;
            Load();
            IsDarkMode = Preferences.Get("dark_mode", false);
        }

        private async void Load()
        {
            var savedPin = await _secureStorageService.GetAsync("user_pin");
            IsAuthenticationEnabled = !string.IsNullOrWhiteSpace(savedPin);
        }

        partial void OnIsDarkModeChanged(bool value)
        {
            Preferences.Set("dark_mode", value);
            Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
        }

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

        partial void OnNewPinChanged(string value)
        {
            PinsDoNotMatchErrorVisible = !string.IsNullOrEmpty(ConfirmPin) && value != ConfirmPin;
        }

        partial void OnConfirmPinChanged(string value)
        {
            PinsDoNotMatchErrorVisible = value != NewPin;
        }

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

        [RelayCommand]
        private async Task GoToPrivacyPolicy()
        {
            await Launcher.OpenAsync(new Uri("https://abarnes.app/"));
        }

        [RelayCommand]
        private async Task ExportAllDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                // get all data types from the database
                var allLoads = await _databaseService.GetLoadsAsync();
                var allExpenses = await _databaseService.GetExpensesForLoadAsync(0);
                var allCompanies = await _databaseService.GetCompaniesAsync();
                var userProfile = await _databaseService.GetUserProfileAsync();

                var exportData = new ExportData
                {
                    Loads = allLoads,
                    Expenses = allExpenses.Select(e => new Expense
                    {
                        Id = e.Id,
                        LoadId = e.LoadId,
                        Category = e.Category,
                        Amount = e.Amount,
                        Date = e.Date,
                        Description = e.Description,
                        ReceiptImagePath = e.ReceiptImagePath
                    }).ToList(),
                    Companies = allCompanies,
                    UserProfile = userProfile
                };

                await _jsonService.ExportDataAsync(exportData, $"DriverLogistics_Backup_{DateTime.Now:yyyy-MM-dd}.json");
                await _alertService.DisplayAlert("Success", "All data has been exported.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ImportDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var importedData = await _jsonService.ImportDataAsync();
                if (importedData != null)
                {
                    // import companies
                    var existingCompanyNames = (await _databaseService.GetCompaniesAsync()).Select(c => c.Name).ToHashSet();

                    if (importedData.Companies != null)
                    {
                        foreach (var company in importedData.Companies)
                        {
                            if (!existingCompanyNames.Contains(company.Name))
                            {
                                company.Id = 0;
                                await _databaseService.SaveCompanyAsync(company);
                            }
                        }
                    }

                    // import loads
                    if (importedData.Loads != null)
                    {
                        foreach (var load in importedData.Loads)
                        {
                            load.Id = 0;
                            await _databaseService.SaveLoadAsync(load);
                        }
                    }

                    // import Expenses
                    if (importedData.Expenses != null)
                    {
                        foreach (var expense in importedData.Expenses)
                        {
                            expense.Id = 0;
                            await _databaseService.SaveExpenseAsync(expense);
                        }
                    }

                    // import User Profile
                    if (importedData.UserProfile != null)
                    {
                        await _databaseService.SaveUserProfileAsync(importedData.UserProfile);
                    }

                    await _alertService.DisplayAlert("Success", "Data imported successfully. Please restart the app to see all changes.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
