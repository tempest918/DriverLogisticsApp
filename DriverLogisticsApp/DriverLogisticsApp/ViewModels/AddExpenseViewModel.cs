using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;

namespace DriverLogisticsApp.ViewModels
{
    [QueryProperty(nameof(LoadId), "LoadId")]
    public partial class AddExpenseViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

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

        public ObservableCollection<string> Categories { get; }

        /// <summary>
        /// initialize the view model for adding a new expense
        /// </summary>
        /// <param name="databaseService"></param>
        public AddExpenseViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

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
        /// save the new expense to the database
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveExpenseAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedCategory) || Amount <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Please select a category and enter a valid amount.", "OK");
                return;
            }

            var newExpense = new Expense
            {
                LoadId = this.LoadId,
                Category = this.SelectedCategory,
                Amount = this.Amount,
                Date = this.Date,
                Description = this.Description
            };

            await _databaseService.SaveExpenseAsync(newExpense);

            await Shell.Current.GoToAsync("..");
        }
    }
}