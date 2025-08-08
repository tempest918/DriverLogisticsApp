using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;

namespace DriverLogisticsApp.ViewModels
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IAlertService _alertService;
        private readonly ISecureStorageService _secureStorageService;

        [ObservableProperty]
        private UserProfile _profile;

        [ObservableProperty]
        private string _newPin;

        [ObservableProperty]
        private string _confirmPin;

        [ObservableProperty]
        private bool _isAuthenticationEnabled;

        /// <summary>
        /// initializes the view model with required services
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="alertService"></param>
        public ProfilePageViewModel(IDatabaseService databaseService, IAlertService alertService, ISecureStorageService secureStorageService)
        {
            _databaseService = databaseService;
            _alertService = alertService;
            _secureStorageService = secureStorageService;
        }

        /// <summary>
        /// loads the user profile from the database
        /// </summary>
        /// <returns></returns>
        public async Task LoadProfileAsync()
        {
            Profile = await _databaseService.GetUserProfileAsync();

            var savedPin = await _secureStorageService.GetAsync("user_pin");
            IsAuthenticationEnabled = !string.IsNullOrWhiteSpace(savedPin);
        }

        /// <summary>
        /// enables or disables authentication based on user input
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// saves the user profile after validating the input
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (string.IsNullOrWhiteSpace(Profile.UserName) || string.IsNullOrWhiteSpace(Profile.CompanyName))
            {
                await _alertService.DisplayAlert("Error", "Your name and company name are required.", "OK");
                return;
            }

            await _databaseService.SaveUserProfileAsync(Profile);
            await _alertService.DisplayAlert("Success", "Your profile has been saved.", "OK");
        }

        /// <summary>
        /// sets a new PIN after validating the input
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SetPinAsync()
        {
            if (string.IsNullOrWhiteSpace(NewPin) || string.IsNullOrWhiteSpace(ConfirmPin))
            {
                await _alertService.DisplayAlert("Error", "Please enter and confirm your new PIN.", "OK");
                return;
            }

            if (NewPin != ConfirmPin)
            {
                await _alertService.DisplayAlert("Error", "PINs do not match. Please try again.", "OK");
                return;
            }

            await _secureStorageService.SetAsync("user_pin", NewPin);

            NewPin = string.Empty;
            ConfirmPin = string.Empty;

            await _alertService.DisplayAlert("Success", "Your PIN has been set.", "OK");
        }
    }
}
