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
            var allLoads = await _databaseService.GetLoadsAsync();
            var allExpenses = await _databaseService.GetExpensesForLoadAsync(0);

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
                }).ToList()
            };

            await _jsonService.ExportDataAsync(exportData, $"DriverLogistics_Backup_{DateTime.Now:yyyy-MM-dd}.json");
        }

        /// <summary>
        /// imports data from a JSON file
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ImportDataAsync()
        {
            var importedData = await _jsonService.ImportDataAsync();
            if (importedData != null)
            {
                if (importedData.Loads != null)
                {
                    foreach (var load in importedData.Loads)
                    {
                        load.Id = 0;
                        await _databaseService.SaveLoadAsync(load);
                    }
                }
                if (importedData.Expenses != null)
                {
                    foreach (var expense in importedData.Expenses)
                    {
                        expense.Id = 0;
                        await _databaseService.SaveExpenseAsync(expense);
                    }
                }

                await _alertService.DisplayAlert("Success", "Data imported successfully. Please restart the app to see the changes.", "OK");
            }
        }
    }
}
