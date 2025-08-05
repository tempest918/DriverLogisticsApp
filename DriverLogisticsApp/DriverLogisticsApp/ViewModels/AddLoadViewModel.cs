using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;

namespace DriverLogisticsApp.ViewModels
{
    public partial class AddLoadViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        string loadNumber;

        [ObservableProperty]
        string shipperName;

        [ObservableProperty]
        string consigneeName;

        [ObservableProperty]
        decimal freightRate;

        [ObservableProperty]
        DateTime pickupDate = DateTime.Today;

        [ObservableProperty]
        DateTime deliveryDate = DateTime.Today.AddDays(1);

        /// <summary>
        /// initialize the view model for adding a new load
        /// </summary>
        /// <param name="databaseService"></param>
        public AddLoadViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// save the new load to the database
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveLoadAsync()
        {
            // validation checks
            if (string.IsNullOrWhiteSpace(LoadNumber) || string.IsNullOrWhiteSpace(ShipperName) || FreightRate <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Please fill in all required fields.", "OK");
                return;
            }

            // create load object
            var newLoad = new Load
            {
                LoadNumber = this.LoadNumber,
                ShipperName = this.ShipperName,
                ConsigneeName = this.ConsigneeName,
                FreightRate = this.FreightRate,
                PickupDate = this.PickupDate,
                DeliveryDate = this.DeliveryDate,
                Status = "Active"
            };

            // save to database
            await _databaseService.SaveLoadAsync(newLoad);

            // navigate back to the main page
            await Shell.Current.GoToAsync("..");
        }
    }
}