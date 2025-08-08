using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace DriverLogisticsApp.ViewModels
{
    // pass LoadId as a query parameter to this view model
    [QueryProperty(nameof(LoadId), "LoadId")]

    public partial class LoadDetailsViewModel : ObservableObject
    {
        // disable annoying warnings that are not relevant to this project
#pragma warning disable MVVMTK0034
#pragma warning disable MVVMTK0045

        private readonly IDatabaseService _databaseService;
        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;
        private readonly PdfService _pdfService;

        private List<Models.ExpenseTypes.Expense> _allExpenses;

        [ObservableProperty]
        private int _loadId;

        [ObservableProperty]
        private Load _load;

        [ObservableProperty]
        private string _toolbarActionText;

        [ObservableProperty]
        private bool _isToolbarActionVisible;

        [ObservableProperty]
        private bool _isInvoiceActionVisible;

        [ObservableProperty]
        private ObservableCollection<Models.ExpenseTypes.Expense> _expenses;

        [ObservableProperty]
        private string _expenseSearchText;

        [ObservableProperty]
        private bool _isBusy;

        /// <summary>
        /// initialize the view model for the load details page
        /// </summary>
        /// <param name="databaseService"></param>
        public LoadDetailsViewModel(IDatabaseService databaseService, IAlertService alertService, INavigationService navigationService, PdfService pdfService)
        {
            _databaseService = databaseService;
            _alertService = alertService;
            _navigationService = navigationService;
            _pdfService = pdfService;

            Expenses = new ObservableCollection<Models.ExpenseTypes.Expense>();
            _allExpenses = new List<Models.ExpenseTypes.Expense>();
        }

        #region Load Related Methods
        /// <summary>
        /// invoked when LoadId changes, loads data for the specified load
        /// </summary>
        /// <param name="value"></param>
        async partial void OnLoadIdChanged(int value)
        {
            // Load the full details of the load from the database
            await LoadDataAsync();
        }

        /// <summary>
        /// used by OnLoadIdChanged to load the full details of the load
        /// </summary>
        /// <returns></returns>
        public async Task LoadDataAsync()
        {
            IsBusy = true;

            try
            {
                var load = await _databaseService.GetLoadAsync(LoadId);
                if (load != null)
                {
                    Load = load;

                    // update the toolbar state based on the load status
                    UpdateToolbarState();

                    // load expenses for this load
                    _allExpenses = await _databaseService.GetExpensesForLoadAsync(LoadId);

                    // filter expenses based on the search text
                    FilterExpenses();
                }
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

        /// <summary>
        /// execute the toolbar action based on the current load status
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ToolbarActionAsync()
        {
            if (Load is null) return;

            if (Load.Status == "Planned")
            {
                Load.Status = "In Progress";
                Load.ActualPickupTime = DateTime.Now;
            }
            else if (Load.Status == "In Progress")
            {
                Load.Status = "Completed";
                Load.ActualDeliveryTime = DateTime.Now;
            }

            await _databaseService.SaveLoadAsync(Load);
            UpdateToolbarState();

            await _alertService.DisplayAlert("Success", $"Load status updated to {Load.Status}.", "OK");

            // reload the data to reflect changes
            await LoadDataAsync();

        }

        /// <summary>
        /// update the toolbar state based on the current load status
        /// </summary>
        private void UpdateToolbarState()
        {
            IsToolbarActionVisible = false;
            IsInvoiceActionVisible = false;

            if (Load?.Status == "Planned")
            {
                ToolbarActionText = "Start Load";
                IsToolbarActionVisible = true;
            }
            else if (Load?.Status == "In Progress")
            {
                ToolbarActionText = "Complete Load";
                IsToolbarActionVisible = true;
            }
            else if (Load?.Status == "Completed" || Load?.Status == "Invoiced")
            {
                IsInvoiceActionVisible = true;
            }
        }

        /// <summary>
        /// use add load page to edit the current load
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToEditAsync()
        {
            if (Load is null)
                return;

            // pass load ID as a query parameter to the AddLoadPage
            await _navigationService.NavigateToAsync(nameof(Views.AddLoadPage), new Dictionary<string, object>
            {
                { "LoadId", Load.Id }
            });
        }

        /// <summary>
        /// update the status of the current load
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ChangeStatusAsync()
        {
            if (Load is null) return;

            // Define the status options
            var newStatus = await _alertService.DisplayActionSheet(
                "Change Load Status",
                "Cancel",
                null,
                "Active", "Completed", "Invoiced", "Cancelled");


            if (!string.IsNullOrWhiteSpace(newStatus) && newStatus != "Cancel")
            {
                Load.Status = newStatus;
                await _databaseService.SaveLoadAsync(Load);

            }
        }

        /// <summary>
        /// delete the current load
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task DeleteLoadAsync()
        {
            // ask for confirmation before deleting
            bool confirmed = await _alertService.DisplayAlert("Confirm Delete", $"Are you sure you want to delete Load #{Load.LoadNumber}?", "Yes", "No");
            if (!confirmed)
                return;

            await _databaseService.DeleteLoadAsync(Load);

            // navigate back to the main list
            await _navigationService.GoBackAsync();
        }

        /// <summary>
        /// create a PDF invoice for the current load and share it
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task CreateInvoiceAsync()
        {
            if (Load is null || IsBusy) return;

            IsBusy = true;
            try
            {
                string filePath = string.Empty;

                var userProfile = await _databaseService.GetUserProfileAsync();

                await Task.Run(() =>
                {
                    filePath = _pdfService.CreateInvoicePdf(this.Load, this.Expenses.ToList(), userProfile);
                });

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = $"Invoice {Load.LoadNumber}",
                    File = new ShareFile(filePath)
                });

                Load.Status = "Invoiced";
                await _databaseService.SaveLoadAsync(this.Load);
                OnPropertyChanged(nameof(Load));
                UpdateToolbarState();
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", $"Failed to create invoice: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// used to open the device's map application for navigation to the specified address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task NavigateToAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return;

            try
            {
                var locations = await Geocoding.Default.GetLocationsAsync(address);
                var location = locations?.FirstOrDefault();

                if (location != null)
                {
                    await Map.Default.OpenAsync(location, new MapLaunchOptions
                    {
                        Name = "Load Location",
                        NavigationMode = NavigationMode.Driving
                    });
                }
                else
                {
                    await _alertService.DisplayAlert("Error", "Could not find a location for the provided address.", "OK");
                }
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Could not open map application.", "OK");
                Debug.WriteLine($"Failed to open map: {ex.Message}");
            }
        }

        #endregion

        #region Expense Related Methods

        /// <summary>
        /// monitor changes to the search text and filter expenses accordingly
        /// </summary>
        /// <param name="value"></param>
        partial void OnExpenseSearchTextChanged(string value)
        {
            FilterExpenses();
        }

        /// <summary>
        /// navigate to the Add Expense page, passing the current Load's Id
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToAddExpenseAsync()
        {
            if (Load is null) return;

            await _navigationService.NavigateToAsync(nameof(Views.AddExpensePage), new Dictionary<string, object>
            {
                { "LoadId", Load.Id }
            });
        }

        /// <summary>
        /// load the details of the selected expense
        /// </summary>
        /// <param name="expense"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToExpenseDetailsAsync(Models.ExpenseTypes.Expense expense)
        {
            if (expense is null)
                return;

            await _navigationService.NavigateToAsync(nameof(Views.ExpenseDetailsPage), new Dictionary<string, object>
            {
                { "ExpenseId", expense.Id }
            });
        }

        /// <summary>
        /// filters the expenses based on the search text
        /// </summary>
        private void FilterExpenses()
        {
            var filtered = string.IsNullOrWhiteSpace(ExpenseSearchText)
                ? _allExpenses
                : _allExpenses.Where(e =>
                    e.FormattedDetails.ToLower().Contains(ExpenseSearchText.ToLower()) ||
                    (e.Description != null && e.Description.ToLower().Contains(ExpenseSearchText.ToLower())));

            // Update the UI's collection with the filtered results
            Expenses.Clear();
            foreach (var expense in filtered)
            {
                Expenses.Add(expense);
            }
        }
        #endregion

    }
}