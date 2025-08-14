using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models.ExpenseTypes;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DriverLogisticsApp.ViewModels
{
    public partial class AllExpensesViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Expense> _expenses;

        [ObservableProperty]
        private bool _isBusy;

        public AllExpensesViewModel(IDatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            Expenses = new ObservableCollection<Expense>();
        }

        [RelayCommand]
        private async Task LoadExpensesAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                Expenses.Clear();
                // Passing 0 to get all expenses, including those not associated with a load
                var expenses = await _databaseService.GetExpensesForLoadAsync(0);
                foreach (var expense in expenses)
                {
                    Expenses.Add(expense);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load expenses: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToAddExpenseAsync()
        {
            await _navigationService.NavigateToAsync(nameof(Views.AddExpensePage));
        }

        [RelayCommand]
        private async Task GoToExpenseDetailsAsync(Expense expense)
        {
            if (expense == null) return;

            await _navigationService.NavigateToAsync(nameof(Views.ExpenseDetailsPage), new Dictionary<string, object>
            {
                { "ExpenseId", expense.Id }
            });
        }
    }
}
