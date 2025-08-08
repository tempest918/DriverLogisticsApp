using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using Moq;

namespace DriverLogisticsApp.Tests
{
    [TestClass]
    public class ImportExportViewModelTests
    {
        private Mock<IDatabaseService> _mockDbService;
        private Mock<IAlertService> _mockAlertService;
        private Mock<IJsonImportExportService> _mockJsonService;
        private ImportExportViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDbService = new Mock<IDatabaseService>();
            _mockAlertService = new Mock<IAlertService>();
            _mockJsonService = new Mock<IJsonImportExportService>();
            _viewModel = new ImportExportViewModel(
                _mockDbService.Object,
                _mockAlertService.Object,
                _mockJsonService.Object);
        }

        [TestMethod]
        public async Task ImportDataCommand_WhenFileIsSelected_SavesDataToDatabase()
        {
            // ARRANGE
            var importData = new ExportData
            {
                Loads = new List<Load> { new Load(), new Load() },
                Expenses = new List<Expense> { new Expense() },
                Companies = new List<Company> { new Company() },
                UserProfile = new UserProfile()
            };

            _mockDbService.Setup(db => db.GetCompaniesAsync()).ReturnsAsync(new List<Company>());
            _mockJsonService.Setup(s => s.ImportDataAsync()).ReturnsAsync(importData);

            // ACT
            await _viewModel.ImportDataCommand.ExecuteAsync(null);

            // ASSERT
            _mockDbService.Verify(db => db.SaveCompanyAsync(It.IsAny<Company>()), Times.Once);
            _mockDbService.Verify(db => db.SaveLoadAsync(It.IsAny<Load>()), Times.Exactly(2));
            _mockDbService.Verify(db => db.SaveExpenseAsync(It.IsAny<Expense>()), Times.Once);
            _mockDbService.Verify(db => db.SaveUserProfileAsync(It.IsAny<UserProfile>()), Times.Once);
            _mockAlertService.Verify(a => a.DisplayAlert("Success", "Data imported successfully. Please restart the app to see all changes.", "OK"), Times.Once);
        }

        [TestMethod]
        public async Task ExportAllDataCommand_WhenCalled_ExportsAllData()
        {
            // ARRANGE
            var loads = new List<Load> { new Load() };
            var expenses = new List<DriverLogisticsApp.Models.ExpenseTypes.Expense> { new DriverLogisticsApp.Models.ExpenseTypes.FuelExpense() };
            _mockDbService.Setup(db => db.GetLoadsAsync()).ReturnsAsync(loads);
            _mockDbService.Setup(db => db.GetExpensesForLoadAsync(0)).ReturnsAsync(expenses);

            // ACT
            await _viewModel.ExportAllDataCommand.ExecuteAsync(null);

            // ASSERT
            _mockJsonService.Verify(s => s.ExportDataAsync(It.IsAny<ExportData>(), It.IsAny<string>()), Times.Once);
        }
    }
}
