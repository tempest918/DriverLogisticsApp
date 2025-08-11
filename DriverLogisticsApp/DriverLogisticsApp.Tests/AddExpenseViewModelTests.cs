using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using Moq;

namespace DriverLogisticsApp.Tests
{
    [TestClass]
    public class AddExpenseViewModelTests
    {
        private Mock<IDatabaseService> _mockDbService;
        private Mock<IAlertService> _mockAlertService;
        private Mock<INavigationService> _mockNavigationService;
        private AddExpenseViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDbService = new Mock<IDatabaseService>();
            _mockAlertService = new Mock<IAlertService>();
            _mockNavigationService = new Mock<INavigationService>();
            _viewModel = new AddExpenseViewModel(
                _mockDbService.Object,
                _mockAlertService.Object,
                _mockNavigationService.Object);
        }

        /// <summary>
        /// test to ensure that invalid data does not get saved and an alert is shown
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SaveExpenseCommand_WithInvalidData_ShowsAlertAndDoesNotSave()
        {
            // ARRANGE
            _viewModel.SelectedCategory = "";

            // ACT
            await _viewModel.SaveExpenseCommand.ExecuteAsync(null);

            // ASSERT
            _mockAlertService.Verify(s => s.DisplayAlert("Error", "Please fill in all required fields.", "OK"), Times.Once);
            _mockDbService.Verify(db => db.SaveExpenseAsync(It.IsAny<Expense>()), Times.Never);
        }
        /// <summary>
        /// test to ensure that valid data gets saved and navigation goes back
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SaveExpenseCommand_WithValidData_SavesAndNavigatesBack()
        {
            // ARRANGE
            _viewModel.SelectedCategory = "Fuel";
            _viewModel.Amount = 50.00m;

            // ACT
            await _viewModel.SaveExpenseCommand.ExecuteAsync(null);

            // ASSERT
            _mockDbService.Verify(db => db.SaveExpenseAsync(It.IsAny<Expense>()), Times.Once);
            _mockNavigationService.Verify(nav => nav.GoBackAsync(), Times.Once);
        }
    }
}
