using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using Moq;
using ExpenseBase = DriverLogisticsApp.Models.ExpenseTypes.Expense;
using FuelExpense = DriverLogisticsApp.Models.ExpenseTypes.FuelExpense;
using GeneralExpense = DriverLogisticsApp.Models.ExpenseTypes.GeneralExpense;

namespace DriverLogisticsApp.Tests
{
    [TestClass]
    public class SettlementReportViewModelTests
    {
        private Mock<IDatabaseService> _mockDbService;
        private Mock<PdfService> _mockPdfService;
        private Mock<IAlertService> _mockAlertService;
        private SettlementReportViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDbService = new Mock<IDatabaseService>();
            _mockPdfService = new Mock<PdfService>();
            _mockAlertService = new Mock<IAlertService>();
            _viewModel = new SettlementReportViewModel(
                _mockDbService.Object,
                _mockPdfService.Object,
                _mockAlertService.Object
                );
        }

        [TestMethod]
        public async Task GenerateReportAsync_CalculatesTotalsAndGroupsCorrectly()
        {
            // ARRANGE
            var startDate = new DateTime(2025, 8, 1);
            var endDate = new DateTime(2025, 8, 10);

            var loads = new List<Load>
            {
                new Load { Id = 1, LoadNumber = "L1", FreightRate = 1000, DeliveryDate = new DateTime(2025, 8, 5) },
                new Load { Id = 2, LoadNumber = "L2", FreightRate = 1500, DeliveryDate = new DateTime(2025, 8, 8) },
                new Load { Id = 3, LoadNumber = "L3", FreightRate = 500, DeliveryDate = new DateTime(2025, 7, 30) }
            };

            var expenses = new List<ExpenseBase>
            {
                new FuelExpense { LoadId = 1, Amount = 100, Date = new DateTime(2025, 8, 4), Category = "Fuel" },
                new GeneralExpense { LoadId = 1, Amount = 25, Date = new DateTime(2025, 8, 4), Category = "Toll" },
                new FuelExpense { LoadId = 2, Amount = 150, Date = new DateTime(2025, 8, 7), Category = "Fuel" },
                new FuelExpense { LoadId = 3, Amount = 50, Date = new DateTime(2025, 7, 29), Category = "Fuel" }
            };

            _viewModel.StartDate = startDate;
            _viewModel.EndDate = endDate;

            _mockDbService.Setup(db => db.GetLoadsAsync()).ReturnsAsync(loads);
            _mockDbService.Setup(db => db.GetExpensesForLoadAsync(0)).ReturnsAsync(expenses);

            // ACT
            await _viewModel.GenerateReportCommand.ExecuteAsync(null);

            // ASSERT
            Assert.AreEqual(2500m, _viewModel.TotalRevenue);
            Assert.AreEqual(275m, _viewModel.TotalExpenses);
            Assert.AreEqual(2225m, _viewModel.NetPay);

            Assert.AreEqual(2, _viewModel.GroupedExpenses.Count);
            Assert.AreEqual(1, _viewModel.DeductionSummaries.Count(s => s.CategoryName == "Fuel"));
            Assert.AreEqual(250m, _viewModel.DeductionSummaries.First(s => s.CategoryName == "Fuel").TotalAmount);
        }
    }
}
