using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;

namespace DriverLogisticsApp.ViewModels
{
    [QueryProperty(nameof(LoadId), "LoadId")]
    public partial class AddLoadViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private int _loadId;

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

        [ObservableProperty]
        private string _title;

        /// <summary>
        /// initialize the view model for adding a new load
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="alertService"></param>
        /// <param name="navigationService"></param>
        public AddLoadViewModel(IDatabaseService databaseService , IAlertService alertService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _alertService = alertService;
            _navigationService = navigationService;

            Title = "Add New Load";
        }

        /// <summary>
        /// automatically adjust the delivery date to one day after the pickup if the pickup is greater than the delivery
        /// </summary>
        /// <param name="value"></param>
        partial void OnPickupDateChanged(DateTime value)
        {

            if (value > DeliveryDate)
            {
                DeliveryDate = value.AddDays(1);
            }
        }

        /// <summary>
        /// validate the dates when changing delivery date
        /// </summary>
        /// <param name="value"></param>
        partial void OnDeliveryDateChanged(DateTime value)
        {
            // if delivery is less than pickup throw an error
            if (value < PickupDate)
            {
                // show alert
                _alertService.DisplayAlert("Invalid Date", "Delivery date cannot be before the pickup date.", "OK");

                // reset date
                DeliveryDate = PickupDate.AddDays(1);
            }
        }

        /// <summary>
        /// load editing data if LoadId is provided
        /// </summary>
        /// <returns></returns>
        public async Task LoadLoadForEditAsync()
        {
            Title = "Edit Load";

            if (LoadId > 0)
            {
                var load = await _databaseService.GetLoadAsync(LoadId);
                if (load != null)
                {
                    // Populate the form fields with the existing data
                    LoadNumber = load.LoadNumber;
                    ShipperName = load.ShipperName;
                    ConsigneeName = load.ConsigneeName;
                    FreightRate = load.FreightRate;
                    PickupDate = load.PickupDate;
                    DeliveryDate = load.DeliveryDate;
                }
            }
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
                await _alertService.DisplayAlert("Error", "Please fill in all required fields.", "OK");
                return;
            }

            // create load object
            var newLoad = new Load
            {
                Id = this.LoadId,
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
            await _navigationService.GoBackAsync();
        }
    }
}