using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Services;
using System.Threading.Tasks;

namespace DriverLogisticsApp.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        private readonly IAlertService _alertService;
        private readonly ISecureStorageService _secureStorageService;

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

        public SettingsPageViewModel(IAlertService alertService, ISecureStorageService secureStorageService)
        {
            _alertService = alertService;
            _secureStorageService = secureStorageService;
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
    }
}
