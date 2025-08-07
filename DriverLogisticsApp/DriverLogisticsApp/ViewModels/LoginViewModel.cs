using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Services;

namespace DriverLogisticsApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAlertService _alertService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _pin;

        public LoginViewModel(IAlertService alertService, ISecureStorageService secureStorageService, INavigationService navigationService)
        {
            _alertService = alertService;
            _secureStorageService = secureStorageService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                var savedPin = await _secureStorageService.GetAsync("user_pin");

                if (!string.IsNullOrWhiteSpace(Pin) && Pin == savedPin)
                {
                    // use service helper to make sure login page doesn't break when making changes
                    var appShell = ServiceHelper.Services.GetService<AppShell>();
                    var app = Application.Current;

                    // load app shell
                    if (appShell != null && app != null && app.Windows.Count > 0)
                    {
                        var window = app.Windows[0];
                        if (window != null)
                        {
                            window.Page = appShell;
                        }
                    }
                    else
                    {
                        await _alertService.DisplayAlert("Error", "Unable to load main application shell.", "OK");
                    }
                }
                else
                {
                    await _alertService.DisplayAlert("Error", "Incorrect PIN. Please try again.", "OK");
                    Pin = string.Empty;
                }
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}