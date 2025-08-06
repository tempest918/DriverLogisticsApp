using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace DriverLogisticsApp.ViewModels
{
    // pass LoadId as a query parameter to this view model
    [QueryProperty(nameof(LoadId), "LoadId")]

    public partial class LoadDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private List<Models.ExpenseTypes.Expense> _allExpenses;

        [ObservableProperty]
        private int _loadId;

        [ObservableProperty]
        private Load _load;

        [ObservableProperty]
        private ObservableCollection<Models.ExpenseTypes.Expense> _expenses;

        [ObservableProperty]
        private string _expenseSearchText;

        /// <summary>
        /// initialize the view model for the load details page
        /// </summary>
        /// <param name="databaseService"></param>
        public LoadDetailsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _expenses = new ObservableCollection<Models.ExpenseTypes.Expense>();
            _allExpenses = new List<Models.ExpenseTypes.Expense>();
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
                    _allExpenses = await _databaseService.GetExpensesForLoadAsync(LoadId);

                    // filter expenses based on the search text
                    FilterExpenses();
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

            await Shell.Current.GoToAsync(nameof(Views.AddExpensePage), new Dictionary<string, object>
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

            await Shell.Current.GoToAsync(nameof(Views.AddExpensePage), new Dictionary<string, object>
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