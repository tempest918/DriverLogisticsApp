using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using Moq;
using ExpenseBase = DriverLogisticsApp.Models.ExpenseTypes.Expense;

namespace DriverLogisticsApp.Tests
{
    [TestClass]
    public class LoadDetailsViewModelTests
    {
        private Mock<IDatabaseService> _mockDbService;
        private Mock<IAlertService> _mockAlertService;
        private Mock<INavigationService> _mockNavigationService;
        private LoadDetailsViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDbService = new Mock<IDatabaseService>();
            _mockAlertService = new Mock<IAlertService>();
            _mockNavigationService = new Mock<INavigationService>();
            _viewModel = new LoadDetailsViewModel(
                _mockDbService.Object,
                _mockAlertService.Object,
                _mockNavigationService.Object);
        }

        /// <summary>
        /// tests that when DeleteLoadCommand is executed and the user confirms the deletion, the load is deleted from the database and the app navigates back to the previous page.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DeleteLoadCommand_WhenConfirmed_DeletesAndNavigatesBack()
        {
            // ARRANGE
            _viewModel.Load = new Load { Id = 1, LoadNumber = "Test" };
            _mockAlertService.Setup(s => s.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), "Yes", "No")).ReturnsAsync(true);

            // ACT
            await _viewModel.DeleteLoadCommand.ExecuteAsync(null);

            // ASSERT
            _mockDbService.Verify(db => db.DeleteLoadAsync(It.IsAny<Load>()), Times.Once);
            _mockNavigationService.Verify(nav => nav.GoBackAsync(), Times.Once);
        }

        /// <summary>
        /// tests that when GoToEditCommand is executed, the app navigates to the AddLoadPage with the correct LoadId parameter.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GoToEditCommand_Navigates_WithCorrectLoadId()
        {
            // ARRANGE
            _viewModel.Load = new Load { Id = 123 };

            // ACT
            await _viewModel.GoToEditCommand.ExecuteAsync(null);

            // ASSERT
            _mockNavigationService.Verify(nav => nav.NavigateToAsync(
                nameof(Views.AddLoadPage),
                It.Is<IDictionary<string, object>>(d => (int)d["LoadId"] == 123)),
                Times.Once);
        }
    }
}
