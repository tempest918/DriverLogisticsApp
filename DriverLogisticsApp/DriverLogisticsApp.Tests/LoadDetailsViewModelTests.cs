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
        private Mock<PdfService> _mockPdfService;
        private LoadDetailsViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDbService = new Mock<IDatabaseService>();
            _mockAlertService = new Mock<IAlertService>();
            _mockNavigationService = new Mock<INavigationService>();
            _mockPdfService = new Mock<PdfService>();
            _viewModel = new LoadDetailsViewModel(
                _mockDbService.Object,
                _mockAlertService.Object,
                _mockNavigationService.Object,
                _mockPdfService.Object
            );
        }

        /// <summary>
        /// tests that when DeleteLoadCommand is executed and the user confirms the deletion, the load is deleted from the database and the app navigates back to the previous page.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DeleteLoadCommand_WhenConfirmed_CancelsLoadAndNavigatesBack()
        {
            // ARRANGE
            var testLoad = new Load { Id = 1, LoadNumber = "Test", IsCancelled = false };
            _viewModel.Load = testLoad;
            _mockAlertService.Setup(s => s.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), "Yes, Cancel Load", "No")).ReturnsAsync(true);

            // ACT
            await _viewModel.DeleteLoadCommand.ExecuteAsync(null);

            // ASSERT
            _mockDbService.Verify(db => db.SaveLoadAsync(It.Is<Load>(l => l.IsCancelled == true)), Times.Once);
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

        /// <summary>
        /// tests that when ChangeStatusCommand is executed, it prompts the user to select a new status and updates the load status accordingly.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ToolbarActionCommand_WhenLoadIsPlanned_UpdatesStatusAndPickupTime()
        {
            // ARRANGE
            var testLoad = new Load { Status = "Planned" };
            _viewModel.Load = testLoad;

            // ACT
            await _viewModel.ToolbarActionCommand.ExecuteAsync(null);

            // ASSERT
            Assert.AreEqual("In Progress", _viewModel.Load.Status);
            Assert.IsNotNull(_viewModel.Load.ActualPickupTime);
            _mockDbService.Verify(db => db.SaveLoadAsync(testLoad), Times.Once);
        }

        /// <summary>
        /// tests that when the toolbar action command is executed while the load status is "In Progress", it updates the status to "Completed" and sets the actual delivery time.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ToolbarActionCommand_WhenLoadIsInProgress_UpdatesStatusAndDeliveryTime()
        {
            // ARRANGE
            var testLoad = new Load { Status = "In Progress" };
            _viewModel.Load = testLoad;

            // ACT
            await _viewModel.ToolbarActionCommand.ExecuteAsync(null);

            // ASSERT
            Assert.AreEqual("Completed", _viewModel.Load.Status);
            Assert.IsNotNull(_viewModel.Load.ActualDeliveryTime);
            _mockDbService.Verify(db => db.SaveLoadAsync(testLoad), Times.Once);
        }

        /// <summary>
        /// tests that when the load status is "Planned", the toolbar action is set to "Start Load" and is visible.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateToolbarState_WhenLoadIsPlanned_SetsCorrectState()
        {
            // ARRANGE
            var testLoad = new Load { Id = 1, Status = "Planned" };
            _viewModel.LoadId = testLoad.Id;
            _mockDbService.Setup(db => db.GetLoadAsync(testLoad.Id)).ReturnsAsync(testLoad);

            // ACT
            await _viewModel.LoadDataAsync();

            // ASSERT
            Assert.AreEqual("Start Load", _viewModel.ToolbarActionText);
            Assert.IsTrue(_viewModel.IsToolbarActionVisible);
        }

        /// <summary>
        /// tests that when the load status is "Completed", the toolbar action is hidden.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateToolbarState_WhenLoadIsCompleted_HidesToolbarAction()
        {
            // ARRANGE
            var testLoad = new Load { Id = 1, Status = "Completed" };
            _viewModel.LoadId = testLoad.Id;
            _mockDbService.Setup(db => db.GetLoadAsync(testLoad.Id)).ReturnsAsync(testLoad);

            // ACT
            await _viewModel.LoadDataAsync();

            // ASSERT
            Assert.IsFalse(_viewModel.IsToolbarActionVisible);
        }

    }
}
