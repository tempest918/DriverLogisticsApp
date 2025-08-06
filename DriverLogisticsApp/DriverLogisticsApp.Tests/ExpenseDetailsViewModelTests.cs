using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using Moq;
using ExpenseBase = DriverLogisticsApp.Models.ExpenseTypes.Expense;
using FuelExpense = DriverLogisticsApp.Models.ExpenseTypes.FuelExpense;

namespace DriverLogisticsApp.Tests
{
    [TestClass]
    public class ExpenseDetailsViewModelTests
    {
        private Mock<IDatabaseService> _mockDbService;
        private Mock<IAlertService> _mockAlertService;
        private Mock<INavigationService> _mockNavigationService;
        private ExpenseDetailsViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDbService = new Mock<IDatabaseService>();
            _mockAlertService = new Mock<IAlertService>();
            _mockNavigationService = new Mock<INavigationService>();
            _viewModel = new ExpenseDetailsViewModel(
                _mockDbService.Object,
                _mockAlertService.Object,
                _mockNavigationService.Object);
        }

        /// <summary>
        /// tests that when DeleteExpenseCommand is executed and the user confirms, the expense is deleted and navigation goes back
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DeleteExpenseCommand_WhenConfirmed_DeletesAndNavigatesBack()
        {
            // ARRANGE
            _viewModel.Expense = new FuelExpense { Id = 1, Category = "Fuel" };
            _mockAlertService.Setup(s => s.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), "Yes", "No")).ReturnsAsync(true);

            // ACT
            await _viewModel.DeleteExpenseCommand.ExecuteAsync(null);

            // ASSERT
            _mockDbService.Verify(db => db.DeleteExpenseAsync(It.Is<Expense>(e => e.Id == 1)), Times.Once);
            _mockNavigationService.Verify(nav => nav.GoBackAsync(), Times.Once);
        }

        /// <summary>
        /// tests that GoToEditExpenseCommand navigates to AddExpensePage with the correct ExpenseId parameter
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GoToEditExpenseCommand_Navigates_WithCorrectExpenseId()
        {
            // ARRANGE
            _viewModel.Expense = new FuelExpense { Id = 456 };

            // ACT
            await _viewModel.GoToEditExpenseCommand.ExecuteAsync(null);

            // ASSERT
            _mockNavigationService.Verify(nav => nav.NavigateToAsync(
                nameof(Views.AddExpensePage),
                It.Is<IDictionary<string, object>>(d => (int)d["ExpenseId"] == 456)),
                Times.Once);
        }
    }
}
