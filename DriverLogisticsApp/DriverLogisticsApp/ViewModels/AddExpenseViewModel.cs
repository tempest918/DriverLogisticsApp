using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;

namespace DriverLogisticsApp.ViewModels
{
    [QueryProperty(nameof(LoadId), "LoadId")]
    [QueryProperty(nameof(ExpenseId), "ExpenseId")]

    public partial class AddExpenseViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private int _loadId;

        [ObservableProperty]
        private string _selectedCategory;

        [ObservableProperty]
        private decimal _amount;

        [ObservableProperty]
        private DateTime _date = DateTime.Today;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private int _expenseId;

        [ObservableProperty]
        private string _title = "Add Expense";

        public ObservableCollection<string> Categories { get; }

        /// <summary>
        /// initialize the view model for adding a new expense
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="alertService"></param>
        /// <param name="navigationService"></param>
        public AddExpenseViewModel(IDatabaseService databaseService, IAlertService alertService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _alertService = alertService;
            _navigationService = navigationService;

            Categories = new ObservableCollection<string>
            {
                "Fuel",
                "Toll",
                "Maintenance",
                "Food",
                "Other"
            };
        }

        /// <summary>
        /// loads the expense data for editing when ExpenseId changes
        /// </summary>
        /// <param name="value"></param>
        async partial void OnExpenseIdChanged(int value)
        {
            if (value > 0)
            {
                await LoadExpenseForEditAsync();
            }
        }

        /// <summary>
        /// loads the expense data for editing if ExpenseId is provided
        /// </summary>
        /// <returns></returns>
        public async Task LoadExpenseForEditAsync()
        {
            if (ExpenseId > 0)
            {
                Title = "Edit Expense";
                var expense = await _databaseService.GetSimpleExpenseAsync(ExpenseId);
                if (expense != null)
                {
                    LoadId = expense.LoadId;
                    SelectedCategory = expense.Category;
                    Amount = expense.Amount;
                    Date = expense.Date;
                    Description = expense.Description;
                }
            }
        }

        /// <summary>
        /// save the new expense to the database
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveExpenseAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedCategory) || Amount <= 0)
            {
                await _alertService.DisplayAlert("Error", "Please select a category and enter a valid amount.", "OK");
                return;
            }

            var expenseToSave = new Models.Expense
            {
                Id = this.ExpenseId,
                LoadId = this.LoadId,
                Category = this.SelectedCategory,
                Amount = this.Amount,
                Date = this.Date,
                Description = this.Description
            };

            await _databaseService.SaveExpenseAsync(expenseToSave);

            await _navigationService.GoBackAsync();
        }
    }
}