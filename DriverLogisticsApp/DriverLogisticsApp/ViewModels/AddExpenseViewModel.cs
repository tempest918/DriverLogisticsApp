using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;

namespace DriverLogisticsApp.ViewModels
{
    // disable annoying warnings that are not relevant to this project
#pragma warning disable MVVMTK0034
#pragma warning disable MVVMTK0045

    [QueryProperty(nameof(LoadId), "LoadId")]
    [QueryProperty(nameof(ExpenseId), "ExpenseId")]

    public partial class AddExpenseViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private int? _loadId;

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

        [ObservableProperty]
        private string _receiptImagePath;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _categoryErrorVisible;
        [ObservableProperty]
        private bool _amountErrorVisible;

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
            else
            {
                Title = "Add Expense";
                LoadId = null;
            }
        }

        /// <summary>
        /// loads the expense data for editing if ExpenseId is provided
        /// </summary>
        /// <returns></returns>
        public async Task LoadExpenseForEditAsync()
        {
            if (IsBusy) return;

            if (ExpenseId > 0)
            {
                Title = "Edit Expense";

                IsBusy = true;
                try
                {

                    var expense = await _databaseService.GetSimpleExpenseAsync(ExpenseId);
                    if (expense != null)
                    {
                        LoadId = expense.LoadId;
                        SelectedCategory = expense.Category;
                        Amount = expense.Amount;
                        Date = expense.Date;
                        Description = expense.Description;
                        ReceiptImagePath = expense.ReceiptImagePath;
                    }
                }
                catch (Exception ex)
                {
                    await _alertService.DisplayAlert("Error", $"Failed to save expense: {ex.Message}", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        /// <summary>
        /// saves the photo taken with the camera and updates the ReceiptImagePath property
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task AttachPhotoAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                // capture photo using the device camera
                var result = await MediaPicker.Default.CapturePhotoAsync();

                // save the file into local storage
                if (result != null)
                {
                    var newFilePath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
                    using (var stream = await result.OpenReadAsync())
                    using (var newStream = File.OpenWrite(newFilePath))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    ReceiptImagePath = newFilePath;
                }
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", $"Failed to attach photo: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// save the new expense to the database
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveExpenseAsync()
        {
            if (IsBusy) return;

            if (!ValidateForm())
            {
                await _alertService.DisplayAlert("Error", "Please fill in all required fields.", "OK");
                return;
            }

            IsBusy = true;
            try
            {

                var expenseToSave = new Models.Expense
                {
                    Id = this.ExpenseId,
                    LoadId = this.LoadId,
                    Category = this.SelectedCategory,
                    Amount = this.Amount,
                    Date = this.Date,
                    Description = this.Description,
                    ReceiptImagePath = this.ReceiptImagePath
                };

                await _databaseService.SaveExpenseAsync(expenseToSave);

                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", $"Failed to save expense: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }

        }

        /// <summary>
        /// validates the form inputs and sets error visibility flags
        /// </summary>
        /// <returns></returns>
        private bool ValidateForm()
        {
            CategoryErrorVisible = string.IsNullOrWhiteSpace(SelectedCategory);
            AmountErrorVisible = Amount <= 0;

            return !(CategoryErrorVisible || AmountErrorVisible);
        }

    }
}