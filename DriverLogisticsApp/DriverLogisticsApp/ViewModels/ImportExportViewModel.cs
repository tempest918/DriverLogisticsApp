using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;

namespace DriverLogisticsApp.ViewModels
{
    public partial class ImportExportViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IAlertService _alertService;
        private readonly IJsonImportExportService _jsonService;

        [ObservableProperty]
        private bool _isBusy;

        /// <summary>
        /// initializes the import/export view model
        /// </summary>
        public ImportExportViewModel(IDatabaseService databaseService, IAlertService alertService, IJsonImportExportService jsonService)
        {
            _databaseService = databaseService;
            _alertService = alertService;
            _jsonService = jsonService;
        }

        /// <summary>
        /// Exports all data to JSON file
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ExportAllDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                // get all data types from the database
                var allLoads = await _databaseService.GetLoadsAsync();
                var allExpenses = await _databaseService.GetExpensesForLoadAsync(0);
                var allCompanies = await _databaseService.GetCompaniesAsync();
                var userProfile = await _databaseService.GetUserProfileAsync();

                var exportData = new ExportData
                {
                    Loads = allLoads,
                    Expenses = allExpenses.Select(e => new Expense
                    {
                        Id = e.Id,
                        LoadId = e.LoadId,
                        Category = e.Category,
                        Amount = e.Amount,
                        Date = e.Date,
                        Description = e.Description,
                        ReceiptImagePath = e.ReceiptImagePath
                    }).ToList(),
                    Companies = allCompanies,
                    UserProfile = userProfile
                };

                await _jsonService.ExportDataAsync(exportData, $"DriverLogistics_Backup_{DateTime.Now:yyyy-MM-dd}.json");
                await _alertService.DisplayAlert("Success", "All data has been exported.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// imports data from a JSON file
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ImportDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var importedData = await _jsonService.ImportDataAsync();
                if (importedData != null)
                {
                    // import companies
                    var existingCompanyNames = (await _databaseService.GetCompaniesAsync()).Select(c => c.Name).ToHashSet();

                    if (importedData.Companies != null)
                    {
                        foreach (var company in importedData.Companies)
                        {
                            if (!existingCompanyNames.Contains(company.Name))
                            {
                                company.Id = 0;
                                await _databaseService.SaveCompanyAsync(company);
                            }
                        }
                    }

                    // import loads
                    if (importedData.Loads != null)
                    {
                        foreach (var load in importedData.Loads)
                        {
                            load.Id = 0;
                            await _databaseService.SaveLoadAsync(load);
                        }
                    }

                    // import Expenses
                    if (importedData.Expenses != null)
                    {
                        foreach (var expense in importedData.Expenses)
                        {
                            expense.Id = 0;
                            await _databaseService.SaveExpenseAsync(expense);
                        }
                    }

                    // import User Profile
                    if (importedData.UserProfile != null)
                    {
                        await _databaseService.SaveUserProfileAsync(importedData.UserProfile);
                    }

                    await _alertService.DisplayAlert("Success", "Data imported successfully. Please restart the app to see all changes.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
