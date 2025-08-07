using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
