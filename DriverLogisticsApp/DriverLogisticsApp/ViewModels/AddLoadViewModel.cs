using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;

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

        [ObservableProperty]
        private ObservableCollection<Company> _companyList;
        [ObservableProperty]
        private Company _selectedShipper;
        [ObservableProperty]
        private Company _selectedConsignee;

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

        [ObservableProperty]
        private bool _isBusy;

        /// <summary>
        /// initialize the view model for adding a new load
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="alertService"></param>
        /// <param name="navigationService"></param>
        public AddLoadViewModel(IDatabaseService databaseService, IAlertService alertService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _alertService = alertService;
            _navigationService = navigationService;

            // Initialize the company list
            CompanyList = new ObservableCollection<Company>();
        }

        /// <summary>
        /// load companies and load data if editing an existing load
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            await LoadCompaniesAsync();

            if (LoadId > 0)
            {
                Title = "Edit Company";
                await LoadLoadForEditAsync();
            }
            else
            {
                Title = "Add New Load";
            }
        }

        /// <summary>
        /// get the list of companies from the database and populate the CompanyList collection.
        /// </summary>
        /// <returns></returns>
        public async Task LoadCompaniesAsync()
        {
            CompanyList.Clear();
            var companies = await _databaseService.GetCompaniesAsync();
            foreach (var company in companies)
            {
                CompanyList.Add(company);
            }
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
                    SelectedShipper = CompanyList.FirstOrDefault(c => c.Id == load.ShipperId);
                    SelectedConsignee = CompanyList.FirstOrDefault(c => c.Id == load.ConsigneeId);
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
            if (IsBusy) return;

            // validation checks
            if (string.IsNullOrWhiteSpace(LoadNumber) || FreightRate <= 0)
            {
                await _alertService.DisplayAlert("Error", "Please fill in all required fields.", "OK");
                return;
            }

            if (SelectedShipper is null)
            {
                await _alertService.DisplayAlert("Error", "Please select a shipper.", "OK");
                return;
            }

            IsBusy = true;
            try
            {

                var combinedPickupDateTime = PickupDate.Date + PickupTime;
                var combinedDeliveryDateTime = DeliveryDate.Date + DeliveryTime;

                // create load object
                var newLoad = new Load
                {
                    Id = this.LoadId,
                    LoadNumber = this.LoadNumber,
                    ShipperId = SelectedShipper.Id,
                    ConsigneeId = SelectedConsignee?.Id,
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
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", $"Failed to save load: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}