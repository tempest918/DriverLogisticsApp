using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models.ExpenseTypes;
using DriverLogisticsApp.Services;
using System.Diagnostics;

namespace DriverLogisticsApp.ViewModels
{
    [QueryProperty(nameof(ExpenseId), "ExpenseId")]
    public partial class ExpenseDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int _expenseId;

        [ObservableProperty]
        private Expense _expense;


        /// <summary>
        /// initialize the view model for the expense details page
        /// </summary>
        /// <param name="databaseService"></param>
        public ExpenseDetailsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// reloads the expense data when ExpenseId changes
        /// </summary>
        /// <param name="value"></param>
        partial void OnExpenseIdChanged(int value)
        {
            LoadDataAsync();
        }

        /// <summary>
        /// reloads the expense data from the database
        /// </summary>
        /// <returns></returns>
        public async Task LoadDataAsync()
        {
            try
            {
                var expense = await _databaseService.GetExpenseAsync(ExpenseId);
                if (expense != null)
                {
                    Expense = expense;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load expense data: {ex.Message}");
            }
        }

        /// <summary>
        /// edits the current expense by navigating to the AddExpensePage
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToEditExpenseAsync()
        {
            if (Expense is null) return;

            // pass the ExpenseId as a query parameter to the AddExpensePage
            await Shell.Current.GoToAsync(nameof(Views.AddExpensePage), new Dictionary<string, object>
            {
                { "ExpenseId", Expense.Id }
            }); 
        }

        /// <summary>
        /// deletes the current expense after user confirmation
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task DeleteExpenseAsync()
        {
            if (Expense is null) return;

            bool confirmed = await Shell.Current.DisplayAlert("Confirm Delete", "Are you sure you want to delete this expense?", "Yes", "No");
            if (!confirmed) return;

            // convert back to databse model for deletion
            var expenseToDelete = new Models.Expense
            {
                Id = Expense.Id
            };

            await _databaseService.DeleteExpenseAsync(expenseToDelete);

            await Shell.Current.GoToAsync("..");
        }
    }
}