using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DriverLogisticsApp.Models.ExpenseTypes;

namespace DriverLogisticsApp.ViewModels
{
    // pass LoadId as a query parameter to this view model
    [QueryProperty(nameof(LoadId), "LoadId")]

    public partial class LoadDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int _loadId;

        [ObservableProperty]
        private Load _load;

        [ObservableProperty]
        private ObservableCollection<Models.ExpenseTypes.Expense> _expenses;

        /// <summary>
        /// initialize the view model for the load details page
        /// </summary>
        /// <param name="databaseService"></param>
        public LoadDetailsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _expenses = new ObservableCollection<Models.ExpenseTypes.Expense>();
        }

        #region Load Related Methods
        /// <summary>
        /// invoked when LoadId changes, loads data for the specified load
        /// </summary>
        /// <param name="value"></param>
        partial void OnLoadIdChanged(int value)
        {
            // Load the full details of the load from the database
            LoadDataAsync();
        }

        /// <summary>
        /// used by OnLoadIdChanged to load the full details of the load
        /// </summary>
        /// <returns></returns>
        public async Task LoadDataAsync()
        {
            try
            {
                var load = await _databaseService.GetLoadAsync(LoadId);
                if (load != null)
                {
                    Load = load;

                    // load expenses for this load
                    var expensesFromDb = await _databaseService.GetExpensesForLoadAsync(LoadId);

                    Expenses.Clear();
                    foreach (var expense in expensesFromDb)
                    {
                        Expenses.Add(expense);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load data: {ex.Message}");
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
            await Shell.Current.GoToAsync(nameof(Views.AddLoadPage), new Dictionary<string, object>
            {
                { "LoadId", Load.Id }
            });
        }

        /// <summary>
        /// delete the current load
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task DeleteLoadAsync()
        {
            // ask for confirmation before deleting
            bool confirmed = await Shell.Current.DisplayAlert("Confirm Delete", $"Are you sure you want to delete Load #{Load.LoadNumber}?", "Yes", "No");
            if (!confirmed)
                return;

            await _databaseService.DeleteLoadAsync(Load);

            // navigate back to the main list
            await Shell.Current.GoToAsync("..");
        }
        #endregion

        #region Expense Related Methods

        /// <summary>
        /// navigate to the Add Expense page, passing the current Load's Id
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToAddExpenseAsync()
        {
            if (Load is null) return;

            await Shell.Current.GoToAsync(nameof(Views.AddExpensePage), new Dictionary<string, object>
            {
                { "LoadId", Load.Id }
            });
        }
        #endregion

    }
}