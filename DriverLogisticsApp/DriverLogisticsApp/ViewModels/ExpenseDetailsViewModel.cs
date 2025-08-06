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
        private readonly IDatabaseService _databaseService;
        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private int _expenseId;

        [ObservableProperty]
        private Expense _expense;


        /// <summary>
        /// initialize the view model for the expense details page
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="alertService"></param>
        /// <param name="navigationService"></param>
        public ExpenseDetailsViewModel(IDatabaseService databaseService, IAlertService alertService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _alertService = alertService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// reloads the expense data when ExpenseId changes
        /// </summary>
        /// <param name="value"></param>
        async partial void OnExpenseIdChanged(int value)
        {
            await LoadDataAsync();
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
            await _navigationService.NavigateToAsync(nameof(Views.AddExpensePage), new Dictionary<string, object>
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

            bool confirmed = await _alertService.DisplayAlert("Confirm Delete", $"Are you sure you want to delete the expense for {Expense.Category}?", "Yes", "No");
            if (!confirmed) return;

            // convert back to databse model for deletion
            var expenseToDelete = new Models.Expense
            {
                Id = Expense.Id
            };

            await _databaseService.DeleteExpenseAsync(expenseToDelete);

            await _navigationService.GoBackAsync();
        }
    }
}