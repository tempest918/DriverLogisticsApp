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
        private List<Expense> _allExpenses;

        [ObservableProperty]
        private ObservableCollection<Expense> _expenses;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private DateTime _endDate;

        [ObservableProperty]
        private string _selectedExpenseType;

        [ObservableProperty]
        private string _searchText;

        public ObservableCollection<string> ExpenseTypes { get; }

        public AllExpensesViewModel(IDatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            Expenses = new ObservableCollection<Expense>();
            _allExpenses = new List<Expense>();

            var today = DateTime.Today;
            _startDate = new DateTime(today.Year, today.Month, 1);
            _endDate = _startDate.AddMonths(1).AddDays(-1);

            ExpenseTypes = new ObservableCollection<string> { "All", "General Expenses" };
            _selectedExpenseType = "All";
        }

        partial void OnStartDateChanged(DateTime value) => FilterExpenses();
        partial void OnEndDateChanged(DateTime value) => FilterExpenses();
        partial void OnSelectedExpenseTypeChanged(string value) => FilterExpenses();
        partial void OnSearchTextChanged(string value) => FilterExpenses();


        [RelayCommand]
        private async Task LoadExpensesAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                _allExpenses = await _databaseService.GetExpensesForLoadAsync(0);
                FilterExpenses();
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

        private void FilterExpenses()
        {
            if (_allExpenses == null) return;

            var filtered = _allExpenses.AsEnumerable();

            // Date range filter
            filtered = filtered.Where(e => e.Date.Date >= StartDate.Date && e.Date.Date <= EndDate.Date);

            // Expense type filter
            if (SelectedExpenseType == "General Expenses")
            {
                filtered = filtered.Where(e => e is GeneralExpense && !e.LoadId.HasValue);
            }

            // Search text filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(e =>
                    (e.Description != null && e.Description.ToLower().Contains(SearchText.ToLower())) ||
                    (e.Category != null && e.Category.ToLower().Contains(SearchText.ToLower())) ||
                    (e.LoadNumber != null && e.LoadNumber.ToLower().Contains(SearchText.ToLower()))
                );
            }

            Expenses.Clear();
            foreach (var expense in filtered)
            {
                Expenses.Add(expense);
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
