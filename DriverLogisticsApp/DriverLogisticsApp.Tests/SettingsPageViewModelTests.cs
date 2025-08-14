using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using Moq;
using IPreferences = DriverLogisticsApp.Services.IPreferences;

namespace DriverLogisticsApp.Tests
{
    [TestClass]
    public class SettingsPageViewModelTests
    {
        private Mock<IDatabaseService> _mockDbService;
        private Mock<IAlertService> _mockAlertService;
        private Mock<IJsonImportExportService> _mockJsonService;
        private Mock<ISecureStorageService> _mockSecureStorageService;
        private Mock<INavigationService> _mockNavigationService;
        private Mock<IPreferences> _mockPreferences;
        private SettingsPageViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDbService = new Mock<IDatabaseService>();
            _mockAlertService = new Mock<IAlertService>();
            _mockJsonService = new Mock<IJsonImportExportService>();
            _mockSecureStorageService = new Mock<ISecureStorageService>();
            _mockNavigationService = new Mock<INavigationService>();
            _mockPreferences = new Mock<IPreferences>();

            _viewModel = new SettingsPageViewModel(
                _mockAlertService.Object,
                _mockSecureStorageService.Object,
                _mockDbService.Object,
                _mockJsonService.Object,
                _mockNavigationService.Object,
                _mockPreferences.Object
                );
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
