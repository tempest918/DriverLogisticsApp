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
        private string _loadNumber;

        // shipper
        [ObservableProperty]
        private string _shipperName;
        [ObservableProperty]
        private string _shipperAddressLineOne;
        [ObservableProperty]
        private string _shipperAddressLineTwo;
        [ObservableProperty]
        private string _shipperCity;
        [ObservableProperty]
        private string _shipperState;
        [ObservableProperty]
        private string _shipperZipCode;
        [ObservableProperty]
        private string _shipperCountry;
        [ObservableProperty]
        private string _shipperPhoneNumber;

        // consignee
        [ObservableProperty]
        private string _consigneeName;
        [ObservableProperty]
        private string _consigneeAddressLineOne;
        [ObservableProperty]
        private string _consigneeAddressLineTwo;
        [ObservableProperty]
        private string _consigneeCity;
        [ObservableProperty]
        private string _consigneeState;
        [ObservableProperty]
        private string _consigneeZipCode;
        [ObservableProperty]
        private string _consigneeCountry;
        [ObservableProperty]
        private string _consigneePhoneNumber;

        // others load details
        [ObservableProperty]
        private decimal _freightRate;
        [ObservableProperty]
        private DateTime _pickupDate = DateTime.Today;
        [ObservableProperty]
        private DateTime _deliveryDate = DateTime.Today.AddDays(1);
        [ObservableProperty]
        private TimeSpan _pickupTime = DateTime.Now.TimeOfDay;
        [ObservableProperty]
        private TimeSpan _deliveryTime = DateTime.Now.TimeOfDay;
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
                    ShipperAddressLineOne = load.ShipperAddressLineOne;
                    ShipperAddressLineTwo = load.ShipperAddressLineTwo;
                    ShipperCity = load.ShipperCity;
                    ShipperState = load.ShipperState;
                    ShipperZipCode = load.ShipperZipCode;
                    ShipperCountry = load.ShipperCountry;
                    ShipperPhoneNumber = load.ShipperPhoneNumber;

                    ConsigneeName = load.ConsigneeName;
                    ConsigneeAddressLineOne = load.ConsigneeAddressLineOne;
                    ConsigneeAddressLineTwo = load.ConsigneeAddressLineTwo;
                    ConsigneeCity = load.ConsigneeCity;
                    ConsigneeState = load.ConsigneeState;
                    ConsigneeZipCode = load.ConsigneeZipCode;
                    ConsigneeCountry = load.ConsigneeCountry;
                    ConsigneePhoneNumber = load.ConsigneePhoneNumber;

                    FreightRate = load.FreightRate;
                    PickupDate = load.PickupDate.Date;
                    PickupTime = load.PickupDate.TimeOfDay;
                    DeliveryDate = load.DeliveryDate.Date;
                    DeliveryTime = load.DeliveryDate.TimeOfDay;
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

            var combinedPickupDateTime = PickupDate.Date + PickupTime;
            var combinedDeliveryDateTime = DeliveryDate.Date + DeliveryTime;

            // create load object
            var newLoad = new Load
            {
                Id = this.LoadId,
                LoadNumber = this.LoadNumber,
                ShipperName = this.ShipperName,
                ShipperAddressLineOne = this.ShipperAddressLineOne,
                ShipperAddressLineTwo = this.ShipperAddressLineTwo,
                ShipperCity = this.ShipperCity,
                ShipperState = this.ShipperState,
                ShipperZipCode = this.ShipperZipCode,
                ShipperCountry = this.ShipperCountry,
                ShipperPhoneNumber = this.ShipperPhoneNumber,

                ConsigneeName = this.ConsigneeName,
                ConsigneeAddressLineOne = this.ConsigneeAddressLineOne,
                ConsigneeAddressLineTwo = this.ConsigneeAddressLineTwo,
                ConsigneeCity = this.ConsigneeCity,
                ConsigneeState = this.ConsigneeState,
                ConsigneeZipCode = this.ConsigneeZipCode,
                ConsigneeCountry = this.ConsigneeCountry,
                ConsigneePhoneNumber = this.ConsigneePhoneNumber,

                FreightRate = this.FreightRate,
                PickupDate = combinedPickupDateTime,
                DeliveryDate = combinedDeliveryDateTime,
                Status = "Planned"
            };

            // save to database
            await _databaseService.SaveLoadAsync(newLoad);

            // navigate back to the main page
            await _navigationService.GoBackAsync();
        }
    }
}