using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using Moq;

namespace DriverLogisticsApp.Tests
{
    [TestClass]
    public class AddLoadViewModelTests
    {
        private Mock<IDatabaseService> _mockDbService;
        private Mock<IAlertService> _mockAlertService;
        private Mock<INavigationService> _mockNavigationService;
        private AddLoadViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            // mock interfaces
            _mockDbService = new Mock<IDatabaseService>();
            _mockAlertService = new Mock<IAlertService>();
            _mockNavigationService = new Mock<INavigationService>();

            _viewModel = new AddLoadViewModel(_mockDbService.Object, _mockAlertService.Object, _mockNavigationService.Object);
        }

        /// <summary>
        /// test that setting DeliveryDate before PickupDate resets DeliveryDate to PickupDate + 1 day
        /// </summary>
        [TestMethod]
        public void DeliveryDate_WhenSetBeforePickupDate_ResetsToCorrectDate()
        {
            // ARRANGE
            var pickupDate = new DateTime(2025, 8, 10);
            var expectedDeliveryDate = pickupDate.AddDays(1);
            _viewModel.PickupDate = pickupDate;

            // ACT
            _viewModel.DeliveryDate = new DateTime(2025, 8, 9);

            // ASSERT
            Assert.AreEqual(expectedDeliveryDate, _viewModel.DeliveryDate);

            _mockAlertService.Verify(s => s.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// test that changing PickupDate to a later date adjusts DeliveryDate to be at least one day after PickupDate
        /// </summary>
        [TestMethod]
        public void OnPickupDateChanged_WhenNewDateIsLater_AdjustsDeliveryDate()
        {
            // ARRANGE
            _viewModel.DeliveryDate = new DateTime(2025, 8, 10);
            var newPickupDate = new DateTime(2025, 8, 11);
            var expectedDeliveryDate = newPickupDate.AddDays(1);

            // ACT
            _viewModel.PickupDate = newPickupDate;

            // ASSERT
            Assert.AreEqual(expectedDeliveryDate, _viewModel.DeliveryDate);
        }

        /// <summary>
        /// test that SaveLoadAsync with invalid data does not save and shows an alert
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SaveLoadAsync_WithInvalidData_DoesNotSaveAndShowsAlert()
        {
            // ARRANGE
            _viewModel.LoadNumber = ""; // Invalid data

            // ACT
            await _viewModel.SaveLoadCommand.ExecuteAsync(null);

            // ASSERT
            _mockAlertService.Verify(s => s.DisplayAlert("Error", "Please fill in all required fields.", "OK"), Times.Once);
            _mockDbService.Verify(db => db.SaveLoadAsync(It.IsAny<Load>()), Times.Never);
        }

        /// <summary>
        /// test that SaveLoadAsync with valid data saves to database
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SaveLoadAsync_WithValidData_SavesToDatabase()
        {
            // ARRANGE
            _viewModel.SelectedShipper = new Company { Id = 1, Name = "Test Shipper" };
            _viewModel.LoadNumber = "ABC12345";
            _viewModel.FreightRate = 1000;

            // ACT
            await _viewModel.SaveLoadCommand.ExecuteAsync(null);

            // ASSERT
            _mockDbService.Verify(db => db.SaveLoadAsync(It.IsAny<Load>()), Times.Once);
        }
    }
}