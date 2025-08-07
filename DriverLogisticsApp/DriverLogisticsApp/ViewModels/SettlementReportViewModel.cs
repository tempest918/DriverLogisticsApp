using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Storage;
using System.Linq;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace DriverLogisticsApp.ViewModels
{
    public partial class SettlementReportViewModel : ObservableObject
    {
        // disable annoying warnings that are not relevant to this project
        #pragma warning disable MVVMTK0034
        #pragma warning disable MVVMTK0045

        private readonly IDatabaseService _databaseService;
        private readonly PdfService _pdfService;
        private readonly IAlertService _alertService;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today.AddDays(-7);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private decimal _totalRevenue;

        [ObservableProperty]
        private decimal _totalExpenses;

        [ObservableProperty]
        private decimal _netPay;

        [ObservableProperty]
        private bool _isReportGenerated;

        public ObservableCollection<Load> CompletedLoads { get; } = new();

        public ObservableCollection<LoadExpenseGroup> GroupedExpenses { get; } = new();

        public ObservableCollection<CategoryTotal> DeductionSummaries { get; } = new();

        public SettlementReportViewModel(IDatabaseService databaseService, PdfService pdfService, IAlertService alertService)
        {
            _databaseService = databaseService;
            _pdfService = pdfService;
            _alertService = alertService;
            IsReportGenerated = false;
        }

        /// <summary>
        /// generate settlement report for the selected date range.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            // get all loads and filter by date range
            var allLoads = await _databaseService.GetLoadsAsync();
            var loadsInPeriod = allLoads
                .Where(l => l.DeliveryDate.Date >= StartDate.Date && l.DeliveryDate.Date <= EndDate.Date)
                .ToList();

            // get all expenses and filter by date range
            var allExpenses = await _databaseService.GetExpensesForLoadAsync(0);
            var expensesInPeriod = allExpenses
                .Where(e => e.Date.Date >= StartDate.Date && e.Date.Date <= EndDate.Date)
                .ToList();

            // perform calculations
            TotalRevenue = loadsInPeriod.Sum(l => l.FreightRate);
            TotalExpenses = expensesInPeriod.Sum(e => e.Amount);
            NetPay = TotalRevenue - TotalExpenses;

            CompletedLoads.Clear();
            foreach (var load in loadsInPeriod) CompletedLoads.Add(load);

            // group expenses by Load for detailed view
            GroupedExpenses.Clear();
            var expensesGroupedByLoad = expensesInPeriod.GroupBy(e => e.LoadId);
            foreach (var group in expensesGroupedByLoad)
            {
                var load = allLoads.FirstOrDefault(l => l.Id == group.Key);
                var loadNumber = load?.LoadNumber ?? "Unassigned";
                var totalForLoad = group.Sum(e => e.Amount);
                var newGroup = new LoadExpenseGroup(loadNumber, totalForLoad, group.ToList());
                GroupedExpenses.Add(newGroup);
            }

            // group expenses by Category for deduction summary
            DeductionSummaries.Clear();
            var expensesGroupedByCategory = expensesInPeriod.GroupBy(e => e.Category);
            foreach (var group in expensesGroupedByCategory)
            {
                DeductionSummaries.Add(new CategoryTotal
                {
                    CategoryName = group.Key,
                    TotalAmount = group.Sum(e => e.Amount)
                });
            }

            // set the report generated flag
            IsReportGenerated = true;

        }

        /// <summary>
        /// save the generated PDF report to the public Downloads folder.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SavePdfAsync()
        {
            try
            {
                // set filename and create PDF in the cache directory
                var fileName = $"Settlement_{StartDate:yyyy-MM-dd}_to_{EndDate:yyyy-MM-dd}.pdf";
                var tempFilePath = _pdfService.CreateSettlementReportPdf(this, fileName);

                // create a stream from the temp file
                using var stream = File.OpenRead(tempFilePath);

                // use the FileSaver plugin to save to Downloads
                var result = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);

                if (result.IsSuccessful)
                {
                    await _alertService.DisplayAlert("Success", $"Report saved to: {result.FilePath}", "OK");
                }
                else
                {
                    await _alertService.DisplayAlert("Save Failed", "The file was not saved.", "OK");
                }
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// generate a PDF report of the settlement details and open the native share sheet to export it.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ExportPdfAsync()
        {
            var fileName = $"Settlement_{StartDate:yyyy-MM-dd}_to_{EndDate:yyyy-MM-dd}.pdf";

            var filePath = _pdfService.CreateSettlementReportPdf(this, fileName);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Settlement Report {_endDate:yyyy-MM-dd}",
                File = new ShareFile(filePath)
            });
        }
    }
}